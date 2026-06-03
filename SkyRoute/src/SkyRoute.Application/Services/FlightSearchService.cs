using FluentValidation;
using Microsoft.Extensions.Logging;
using SkyRoute.Application.Common;
using SkyRoute.Application.DTOs;
using SkyRoute.Application.Features.Flights;
using SkyRoute.Application.Interfaces;

namespace SkyRoute.Application.Services;

public sealed class FlightSearchService : IFlightSearchService
{
    private readonly IEnumerable<IFlightProvider> _providers;
    private readonly IValidator<FlightSearchRequest> _validator;
    private readonly ILogger<FlightSearchService> _logger;

    public FlightSearchService(
        IEnumerable<IFlightProvider> providers,
        IValidator<FlightSearchRequest> validator,
        ILogger<FlightSearchService> logger)
    {
        _providers = providers;
        _validator = validator;
        _logger = logger;
    }

    public Result<IReadOnlyList<FlightSearchResponse>> Search(FlightSearchRequest request)
    {
        var validation = _validator.Validate(request);

        if (!validation.IsValid)
            return Result<IReadOnlyList<FlightSearchResponse>>.Failure(
                validation.Errors.Select(e => e.ErrorMessage));

        var tz = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);
        var localStart = request.DepartureDate.ToDateTime(TimeOnly.MinValue);
        var localEnd = localStart.AddDays(1);
        var utcStart = TimeZoneInfo.ConvertTimeToUtc(localStart, tz);
        var utcEnd = TimeZoneInfo.ConvertTimeToUtc(localEnd, tz);

        var offers = new List<FlightOffer>();

        foreach (var provider in _providers)
        {
            _logger.LogDebug(
                "Querying {Provider} for flights {Origin}->{Destination}",
                provider.ProviderName, request.OriginAirportCode, request.DestinationAirportCode);

            var providerOffers = provider.Search(request, utcStart, utcEnd);
            offers.AddRange(providerOffers);

            _logger.LogDebug(
                "{Provider} returned {Count} flight(s)", provider.ProviderName, providerOffers.Count);
        }

        _logger.LogInformation(
            "Flight search completed: {Total} offer(s) from {ProviderCount} provider(s)",
            offers.Count, _providers.Count());

        return Result<IReadOnlyList<FlightSearchResponse>>.Success(
            offers.Select(flightOffer => new FlightSearchResponse
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
            }).ToList());
    }
}
