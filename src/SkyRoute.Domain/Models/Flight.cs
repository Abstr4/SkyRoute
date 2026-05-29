namespace SkyRoute.Domain.Models;

public sealed class Flight
{
    public required int Id { get; init; }

    public required string Provider { get; init; }

    public required string FlightNumber { get; init; }

    public required Airport OriginAirport { get; init; }

    public required Airport DestinationAirport { get; init; }

    public required DateTime DepartureTime { get; init; }

    public required DateTime ArrivalTime { get; init; }

    public required CabinClass CabinClass { get; init; }

    public required decimal BaseFare { get; init; }

    public TimeSpan Duration => ArrivalTime - DepartureTime;

    public int DurationMinutes => (int)Duration.TotalMinutes;
}
