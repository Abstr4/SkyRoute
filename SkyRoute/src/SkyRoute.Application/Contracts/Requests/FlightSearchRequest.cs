using SkyRoute.Domain.Models;

namespace SkyRoute.Application.Contracts.Requests;

public sealed record FlightSearchRequest(
    string OriginAirportCode,
    string DestinationAirportCode,
    DateOnly DepartureDate,
    int Passengers,
    CabinClass CabinClass,
    string TimeZone);
