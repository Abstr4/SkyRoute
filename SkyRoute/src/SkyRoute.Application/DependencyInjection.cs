using SkyRoute.Application.Interfaces;
using SkyRoute.Application.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IFlightSearchService, FlightSearchService>();
        services.AddScoped<IBookingService, BookingService>();

        return services;
    }
}
