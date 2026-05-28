namespace SkyRoute.API.Models;

public sealed class Booking
{
    public required int Id { get; set; }
    public required string ReferenceCode { get; init; }
    public required DateTime CreatedAtUtc { get; init; }

    // --- Flattened Flight Snapshot Fields ---
    public required string ProviderName { get; init; }
    public required string FlightNumber { get; init; }
    public required string OriginAirportCode { get; init; }
    public required string DestinationAirportCode { get; init; }
    public required DateTime DepartureTime { get; init; }
    public required DateTime ArrivalTime { get; init; }
    public required CabinClass CabinClass { get; init; }

    // --- Passenger Collection ---
    public required IReadOnlyCollection<Passenger> Passengers { get; init; }

    // --- Final Financial Truth ---
    public required decimal PricePerPassenger { get; init; }
    public required decimal TotalPrice { get; init; }
}