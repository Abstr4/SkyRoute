namespace SkyRoute.Domain.Models;

public sealed class Booking
{
    public int Id { get; set; }
    public string ReferenceCode { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; }

    public string ProviderName { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public string OriginAirportCode { get; set; } = string.Empty;
    public string DestinationAirportCode { get; set; } = string.Empty;
    public DateTimeOffset DepartureTime { get; set; }
    public DateTimeOffset ArrivalTime { get; set; }
    public CabinClass CabinClass { get; set; }

    public List<Passenger> Passengers { get; set; } = [];

    public decimal PricePerPassenger { get; set; }
    public decimal TotalPrice { get; set; }
}
