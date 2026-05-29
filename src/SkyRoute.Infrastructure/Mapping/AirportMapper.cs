using SkyRoute.Application.DTOs;
using SkyRoute.Domain.Models;

namespace SkyRoute.Infrastructure;

public static class AirportMapper
{
    public static AirportDto ToDto(this Airport airport)
    {
        ArgumentNullException.ThrowIfNull(airport);

        return new AirportDto
        {
            Code = airport.Code,
            Name = airport.Name,
            City = airport.City,
            Country = airport.Country,
            CountryCode = airport.CountryCode,
        };
    }
}
