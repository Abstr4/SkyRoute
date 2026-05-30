using SkyRoute.Application.Contracts.Requests;
using SkyRoute.Application.DTOs;

namespace SkyRoute.Application.Interfaces;

public interface IFlightProvider
{
    string ProviderName { get; }

    IReadOnlyCollection<FlightOffer> Search(FlightSearchRequest request);

    FlightOffer? GetByFlightNumber(string flightNumber);
}
