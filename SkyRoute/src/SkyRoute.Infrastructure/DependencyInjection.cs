using Microsoft.EntityFrameworkCore;
using SkyRoute.Application.Interfaces;
using SkyRoute.Infrastructure.Data;
using SkyRoute.Infrastructure.Providers;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<SkyRouteDbContext>(options =>
            options.UseInMemoryDatabase("SkyRoute"));

        services.AddScoped<IFlightProvider, BudgetWingsProvider>();
        services.AddScoped<IFlightProvider, GlobalAirProvider>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        return services;
    }
}
