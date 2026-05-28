using SkyRoute.API.Contracts.Requests;
using SkyRoute.API.Contracts.Responses;
using SkyRoute.API.DTOs;
using SkyRoute.API.Providers;

namespace SkyRoute.API.Services;

public sealed class FlightSearchService
{
    private readonly IEnumerable<IFlightProvider> _providers;
    private readonly FlightOfferRepository _offerRepository;

    public FlightSearchService(IEnumerable<IFlightProvider> providers, FlightOfferRepository offerRepository)
    {
        _providers = providers;
        _offerRepository = offerRepository;
    }

    public IReadOnlyList<FlightSearchResponse> Search(FlightSearchRequest request)
    {
        var offers = new List<FlightOffer>();

        foreach (var provider in _providers)
        {
            var providerOffers = provider.Search(request);
            offers.AddRange(providerOffers);
        }

        // Store offers for later booking validation
        _offerRepository.StoreOffers(offers);

        return offers.Select(flightOffer => new FlightSearchResponse
        {
            FlightId = flightOffer.Id,
            Provider = flightOffer.Provider,
            FlightNumber = flightOffer.FlightNumber,
            OriginAirport =  flightOffer.OriginAirport,
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
