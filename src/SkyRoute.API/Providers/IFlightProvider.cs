using SkyRoute.API.Contracts.Requests;
using SkyRoute.API.DTOs;

namespace SkyRoute.API.Providers;

public interface IFlightProvider
{
    string ProviderName { get; }

    IReadOnlyCollection<FlightOffer> Search(FlightSearchRequest request);
}
