using Microsoft.EntityFrameworkCore;
using SkyRoute.Application.Interfaces;
using SkyRoute.Infrastructure.Data;
using SkyRoute.Infrastructure.Providers;
using SkyRoute.Infrastructure.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<SkyRouteDbContext>(o =>
            o.UseInMemoryDatabase("SkyRoute"));

        services.AddScoped<IBookingRepository, BookingRepository>();

        services.AddScoped<IFlightProvider, BudgetWingsProvider>();
        services.AddScoped<IFlightProvider, GlobalAirProvider>();
        services.AddScoped<IFlightProvider, SuperCheapestProvider>();

        return services;
    }
}
