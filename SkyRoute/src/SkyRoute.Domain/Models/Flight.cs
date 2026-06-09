namespace SkyRoute.Domain.Models;

public sealed class Flight
{
    public int Id { get; set; }

    public string Provider { get; set; } = string.Empty;

    public string FlightNumber { get; set; } = string.Empty;

    public int OriginAirportId { get; set; }

    public Airport OriginAirport { get; set; } = null!;

    public int DestinationAirportId { get; set; }

    public Airport DestinationAirport { get; set; } = null!;

    public DateTimeOffset DepartureTime { get; set; }

    public DateTimeOffset ArrivalTime { get; set; }

    public CabinClass CabinClass { get; set; }

    public decimal BaseFare { get; set; }

    public TimeSpan Duration => ArrivalTime - DepartureTime;

    public int DurationMinutes => (int)Duration.TotalMinutes;
}
