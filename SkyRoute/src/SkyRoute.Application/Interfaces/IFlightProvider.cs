using SkyRoute.Application.DTOs;
using SkyRoute.Application.Features.Flights;

namespace SkyRoute.Application.Interfaces;

public interface IFlightProvider
{
    string ProviderName { get; }

    Task<IReadOnlyCollection<FlightOffer>> SearchAsync(FlightSearchRequest request, DateTimeOffset utcStart, DateTimeOffset utcEnd);

    Task<FlightOffer?> GetByFlightNumberAsync(string flightNumber);
}
