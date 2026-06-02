using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkyRoute.Application.DTOs;
using SkyRoute.Application.Features.Flights;
using SkyRoute.Application.Interfaces;
using SkyRoute.Infrastructure.Data;

namespace SkyRoute.Infrastructure.Providers;

public sealed class GlobalAirProvider : IFlightProvider
{
    private readonly SkyRouteDbContext _context;
    private readonly ILogger<GlobalAirProvider> _logger;

    public GlobalAirProvider(
        SkyRouteDbContext context,
        ILogger<GlobalAirProvider> logger)
    {
        _context = context;
        _logger = logger;
    }

    public string ProviderName => "GlobalAir";

    public async Task<IReadOnlyCollection<FlightOffer>> Search(FlightSearchRequest request, DateTimeOffset utcStart, DateTimeOffset utcEnd)
    {
        var flights = await _context.Flights
            .Where(f => f.Provider == "GlobalAir"
                     && f.OriginAirport.Code == request.OriginAirportCode
                     && f.DestinationAirport.Code == request.DestinationAirportCode
                     && f.DepartureTime >= utcStart
                     && f.DepartureTime < utcEnd
                     && f.DepartureTime > DateTimeOffset.UtcNow)
            .Select(f => new FlightOffer
            {
                Provider = f.Provider,
                FlightNumber = f.FlightNumber,
                OriginAirport = f.OriginAirport.ToDto(),
                DestinationAirport = f.DestinationAirport.ToDto(),
                DepartureTime = f.DepartureTime,
                ArrivalTime = f.ArrivalTime,
                CabinClass = f.CabinClass,
                PricePerPassenger = CalculatePrice(f.BaseFare),
            })
            .ToListAsync();

        _logger.LogDebug(
            "GlobalAir search {Origin}->{Destination}: {Count} result(s)",
            request.OriginAirportCode, request.DestinationAirportCode, flights.Count);

        return flights;
    }

    public async Task<FlightOffer?> GetByFlightNumber(string flightNumber)
    {
        var flight = await _context.Flights
            .FirstOrDefaultAsync(f => f.Provider == "GlobalAir" && f.FlightNumber == flightNumber);

        if (flight is null)
        {
            _logger.LogWarning("GlobalAir flight not found: {FlightNumber}", flightNumber);
            return null;
        }

        _logger.LogDebug("GlobalAir flight found: {FlightNumber}", flightNumber);

        return new FlightOffer
        {
            Provider = flight.Provider,
            FlightNumber = flight.FlightNumber,
            OriginAirport = flight.OriginAirport.ToDto(),
            DestinationAirport = flight.DestinationAirport.ToDto(),
            DepartureTime = flight.DepartureTime,
            ArrivalTime = flight.ArrivalTime,
            CabinClass = flight.CabinClass,
            PricePerPassenger = CalculatePrice(flight.BaseFare),
        };
    }

    private static decimal CalculatePrice(decimal baseFare)
    {
        var finalPrice = baseFare * 1.15m;
        return Math.Round(finalPrice, 2);
    }
}
