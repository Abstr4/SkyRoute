using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using SkyRoute.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddPresentation()
                .AddInfrastructure()
                .AddApplication();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SkyRouteDbContext>();
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();

    var airports = SeedData.CreateAirports();
    db.Airports.AddRange(airports);
    db.SaveChanges();

    var flights = SeedData.CreateFlights(airports);
    db.Flights.AddRange(flights);
    db.SaveChanges();
}

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
    app.UseDeveloperExceptionPage();
}

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCors("Angular");

app.UseAuthorization();

app.MapControllers();

app.Run();
