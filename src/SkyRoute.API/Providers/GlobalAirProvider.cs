using SkyRoute.API.Contracts.Requests;
using SkyRoute.API.Data;
using SkyRoute.API.DTOs;
using SkyRoute.API.Models;

namespace SkyRoute.API.Providers;

public sealed class GlobalAirProvider : IFlightProvider
{
    public string ProviderName => "GlobalAir";

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
                FlightNumber = "GA102",
                OriginAirport = originAirport,
                DestinationAirport = destinationAirport,
                DepartureTime = baseDate.AddHours(7).AddMinutes(15),
                ArrivalTime = baseDate.AddHours(10).AddMinutes(45),
                CabinClass = cabin,
                BaseFare = 120.00m
            },
            new()
            {
                Id = 2,
                Provider = ProviderName,
                FlightNumber = "GA305",
                OriginAirport = originAirport,
                DestinationAirport = destinationAirport,
                DepartureTime = baseDate.AddHours(12).AddMinutes(5),
                ArrivalTime = baseDate.AddHours(15).AddMinutes(25),
                CabinClass = cabin,
                BaseFare = 135.50m
            },
            new()
            {
                Id = 3,
                Provider = ProviderName,
                FlightNumber = "GA412",
                OriginAirport = originAirport,
                DestinationAirport = destinationAirport,
                DepartureTime = baseDate.AddHours(16).AddMinutes(40),
                ArrivalTime = baseDate.AddHours(20).AddMinutes(00),
                CabinClass = cabin,
                BaseFare = 150.00m
            },
            new()
            {
                Id = 4,
                Provider = ProviderName,
                FlightNumber = "GA789",
                OriginAirport = originAirport,
                DestinationAirport = destinationAirport,
                DepartureTime = baseDate.AddHours(21).AddMinutes(30),
                ArrivalTime = baseDate.AddHours(23).AddMinutes(55),
                CabinClass = cabin,
                BaseFare = 99.00m
            }
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
        var finalPrice = baseFare * 1.15m; // 15% fuel surcharge
        return Math.Round(finalPrice, 2);
    }
}
