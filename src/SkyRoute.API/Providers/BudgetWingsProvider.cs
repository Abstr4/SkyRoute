using SkyRoute.API.Contracts.Requests;
using SkyRoute.API.Data;
using SkyRoute.API.DTOs;
using SkyRoute.API.Models;

namespace SkyRoute.API.Providers;

public sealed class BudgetWingsProvider : IFlightProvider
{
    private const decimal MinimumPrice = 29.99m;

    public string ProviderName => "BudgetWings";

    public IReadOnlyCollection<FlightOffer> Search(FlightSearchRequest request)
    {
        var cabin = request.CabinClass;

        var originAirport = MockDataStore.Airports.FirstOrDefault(airport => airport.Code == request.OriginAirportCode);
        var destinationAirport = MockDataStore.Airports.FirstOrDefault(airport => airport.Code == request.DestinationAirportCode);

        if (originAirport == null || destinationAirport == null) return [];

        var baseDate = request.DepartureDate.ToDateTime(new TimeOnly(0, 0), DateTimeKind.Utc);

        var flights = new List<Flight>
        {
            new()
            {
                Id = 1,
                Provider = ProviderName,
                FlightNumber = "BW101",
                OriginAirport = originAirport,
                DestinationAirport = destinationAirport,
                DepartureTime = baseDate.AddHours(6).AddMinutes(40),
                ArrivalTime = baseDate.AddHours(9).AddMinutes(55),
                CabinClass = cabin,
                BaseFare = 64.40m,
            },
            new()
            {
                Id = 2,
                Provider = ProviderName,
                FlightNumber = "BW202",
                OriginAirport = originAirport,
                DestinationAirport = destinationAirport,
                DepartureTime = baseDate.AddHours(18).AddMinutes(10),
                ArrivalTime = baseDate.AddHours(21).AddMinutes(35),
                CabinClass = cabin,
                BaseFare = 54.60m,
            },
            new()
            {
                Id = 3,
                Provider = ProviderName,
                FlightNumber = "BW303",
                OriginAirport = originAirport,
                DestinationAirport = destinationAirport,
                DepartureTime = baseDate.AddHours(20).AddMinutes(50),
                ArrivalTime = baseDate.AddHours(23).AddMinutes(59),
                CabinClass = cabin,
                BaseFare = 49.00m,
            },
        };

        return flights.Select(flight => new FlightOffer
        {
            Id = flight.Id,
            Provider = flight.Provider,
            FlightNumber = flight.FlightNumber,
            OriginAirport = MockDataStore.MapAirportToDto(flight.OriginAirport),
            DestinationAirport = MockDataStore.MapAirportToDto(flight.DestinationAirport),
            DepartureTime = flight.DepartureTime,
            ArrivalTime = flight.ArrivalTime,
            CabinClass = flight.CabinClass,
            PricePerPassenger = CalculatePrice(flight.BaseFare),
        }).ToList();
    }

    private static decimal CalculatePrice(decimal baseFare)
    {
        var discounted = baseFare * 0.90m; // 10% promotional discount
        return Math.Round(Math.Max(discounted, MinimumPrice), 2); // Cap at $29.99 minimum
    }
}
