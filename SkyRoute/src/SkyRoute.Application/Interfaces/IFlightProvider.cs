using SkyRoute.Application.DTOs;
using SkyRoute.Application.Features.Flights;

namespace SkyRoute.Application.Interfaces;

public interface IFlightProvider
{
    string ProviderName { get; }

    Task<IReadOnlyCollection<FlightOffer>> Search(FlightSearchRequest request, DateTimeOffset utcStart, DateTimeOffset utcEnd);

    Task<FlightOffer?> GetByFlightNumber(string flightNumber);
}
