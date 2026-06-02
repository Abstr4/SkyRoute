using SkyRoute.Application.Interfaces;
using SkyRoute.Infrastructure.Providers;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IFlightProvider, BudgetWingsProvider>();
        services.AddScoped<IFlightProvider, GlobalAirProvider>();

        return services;
    }
}
