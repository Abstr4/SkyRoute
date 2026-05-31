using SkyRoute.Application.Contracts.Requests;
using SkyRoute.Application.DTOs;

namespace SkyRoute.Application.Interfaces;

public interface IFlightProvider
{
    string ProviderName { get; }

    IReadOnlyCollection<FlightOffer> Search(FlightSearchRequest request, DateTimeOffset utcStart, DateTimeOffset utcEnd);

    FlightOffer? GetByFlightNumber(string flightNumber);
}
