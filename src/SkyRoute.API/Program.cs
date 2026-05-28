using Scalar.AspNetCore;
using SkyRoute.API.Services;
using SkyRoute.API.Providers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Individual providers so the IEnumerable<IFlightProvider> collection populates
builder.Services.AddScoped<IFlightProvider, BudgetWingsProvider>();
builder.Services.AddScoped<IFlightProvider, GlobalAirProvider>();

// Flight offer repository (singleton to persist offers across requests during booking)
builder.Services.AddSingleton<FlightOfferRepository>();

// Core aggregator service
builder.Services.AddScoped<FlightSearchService>();

// Booking service
builder.Services.AddScoped<BookingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Generates the interactive UI at /scalar/v1
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("SkyRoute Flights API")
            .WithTheme(ScalarTheme.DeepSpace)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
