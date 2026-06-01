using SkyRoute.Application.Common;
using SkyRoute.Application.DTOs;
using SkyRoute.Application.Features.Flights;
using SkyRoute.Application.Interfaces;

namespace SkyRoute.Application.Services;

public sealed class FlightSearchService : IFlightSearchService
{
    private readonly IEnumerable<IFlightProvider> _providers;

    public FlightSearchService(IEnumerable<IFlightProvider> providers)
    {
        _providers = providers;
    }

    public Result<IReadOnlyList<FlightSearchResponse>> Search(FlightSearchRequest request)
    {
        var validator = new FlightSearchRequestValidator();
        var validation = validator.Validate(request);
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
            offers.AddRange(provider.Search(request, utcStart, utcEnd));
        }

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
