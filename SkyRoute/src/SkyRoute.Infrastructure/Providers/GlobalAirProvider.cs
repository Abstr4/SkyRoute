using SkyRoute.Application.Contracts.Requests;
using SkyRoute.Application.DTOs;
using SkyRoute.Application.Interfaces;
using SkyRoute.Infrastructure.Data;

namespace SkyRoute.Infrastructure.Providers;

public sealed class GlobalAirProvider : IFlightProvider
{
    public string ProviderName => "GlobalAir";

    public IReadOnlyCollection<FlightOffer> Search(FlightSearchRequest request)
    {
        return MockDataStore.GlobalAirFlights
            .Where(f => f.OriginAirport.Code == request.OriginAirportCode
                     && f.DestinationAirport.Code == request.DestinationAirportCode)
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
            .ToList();
    }

    public FlightOffer? GetByFlightNumber(string flightNumber)
    {
        var flight = MockDataStore.GlobalAirFlights
            .FirstOrDefault(f => f.FlightNumber == flightNumber);
        if (flight is null) return null;

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
