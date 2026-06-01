using SkyRoute.Application.DTOs;
using SkyRoute.Application.Features.Flights;

namespace SkyRoute.Application.Interfaces;

public interface IFlightProvider
{
    string ProviderName { get; }

    IReadOnlyCollection<FlightOffer> Search(FlightSearchRequest request, DateTimeOffset utcStart, DateTimeOffset utcEnd);

    FlightOffer? GetByFlightNumber(string flightNumber);
}
