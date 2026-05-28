using SkyRoute.API.Models;

namespace SkyRoute.API.Contracts.Requests;

public sealed record FlightSearchRequest(
    string OriginAirportCode,
    string DestinationAirportCode,
    DateOnly DepartureDate,
    int Passengers,
    CabinClass CabinClass);
