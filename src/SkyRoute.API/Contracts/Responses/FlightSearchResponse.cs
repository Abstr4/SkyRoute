using SkyRoute.API.DTOs;
using SkyRoute.API.Models;

namespace SkyRoute.API.Contracts.Responses;

public sealed record FlightSearchResponse
{
    public required int FlightId { get; init; }

    public required string Provider { get; init; }

    public required string FlightNumber { get; init; }

    public required AirportDto OriginAirport { get; init; }

    public required AirportDto DestinationAirport { get; init; }

    public required DateTime DepartureTime { get; init; }

    public required DateTime ArrivalTime { get; init; }

    public required int DurationMinutes { get; init; }

    public required CabinClass CabinClass { get; init; }

    // Pricing details required by the UI
    public required decimal PricePerPassenger { get; init; }

    public required decimal TotalPrice { get; init; }
}
