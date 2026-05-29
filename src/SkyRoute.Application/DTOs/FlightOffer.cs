using SkyRoute.Domain.Models;

namespace SkyRoute.Application.DTOs;

public sealed class FlightOffer
{
    public required string Provider { get; init; }

    public required string FlightNumber { get; init; }

    public required AirportDto OriginAirport { get; init; }

    public required AirportDto DestinationAirport { get; init; }

    public required DateTime DepartureTime { get; init; }

    public required DateTime ArrivalTime { get; init; }

    public required CabinClass CabinClass { get; init; }

    public required decimal PricePerPassenger { get; init; }

    public decimal TotalPrice { get; init; }

    public int DurationMinutes => (int)(ArrivalTime - DepartureTime).TotalMinutes;
}
