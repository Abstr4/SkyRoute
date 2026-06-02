using System.Text.Json.Serialization;
using SkyRoute.API.Exceptions;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddProblemDetails(configure =>
        {
            configure.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddCors(options =>
        {
            options.AddPolicy("Angular", policy =>
            {
                policy
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.AddOpenApi();

        return services;
    }
}
