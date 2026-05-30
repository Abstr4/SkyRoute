using SkyRoute.Application.Contracts.Requests;
using SkyRoute.Application.Contracts.Responses;
using SkyRoute.Application.DTOs;
using SkyRoute.Application.Interfaces;

namespace SkyRoute.Application.Services;

public sealed class FlightSearchService : IFlightSearchService
{
    private readonly IEnumerable<IFlightProvider> _providers;

    public FlightSearchService(IEnumerable<IFlightProvider> providers)
    {
        _providers = providers;
    }

    public IReadOnlyList<FlightSearchResponse> Search(FlightSearchRequest request)
    {
        var offers = new List<FlightOffer>();

        foreach (var provider in _providers)
        {
            offers.AddRange(provider.Search(request));
        }

        return offers.Select(flightOffer => new FlightSearchResponse
        {
            Provider = flightOffer.Provider,
            FlightNumber = flightOffer.FlightNumber,
            OriginAirport = flightOffer.OriginAirport,
            DestinationAirport = flightOffer.DestinationAirport,
            DepartureTime = flightOffer.DepartureTime,
            ArrivalTime = flightOffer.ArrivalTime,
            DurationMinutes = flightOffer.DurationMinutes,
            CabinClass = flightOffer.CabinClass,
            PricePerPassenger = flightOffer.PricePerPassenger,
            TotalPrice = flightOffer.PricePerPassenger * request.Passengers,
        }).ToList();
    }
}
