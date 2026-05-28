using SkyRoute.API.DTOs;
using SkyRoute.API.Models;

namespace SkyRoute.API.Data;

public static class MockDataStore
{
    public static IReadOnlyList<Airport> Airports { get; } = CreateAirports();

    public static readonly List<Booking> Bookings = new();

    public static readonly string[] CabinClasses = ["Economy", "Business", "FirstClass"];

    private static IReadOnlyList<Airport> CreateAirports() =>
    [
        new Airport
        {
            Id = 1,
            Code = "EZE",
            Name = "Ministro Pistarini International Airport",
            City = "Buenos Aires",
            Country = "Argentina",
            CountryCode = "AR",
        },
        new Airport
        {
            Id = 2,
            Code = "COR",
            Name = "Ingeniero Aeronáutico Ambrosio L.V. Taravella International Airport",
            City = "Córdoba",
            Country = "Argentina",
            CountryCode = "AR",
        },
        new Airport
        {
            Id = 3,
            Code = "MDZ",
            Name = "Governor Francisco Gabrielli International Airport",
            City = "Mendoza",
            Country = "Argentina",
            CountryCode = "AR",
        },
        new Airport
        {
            Id = 4,
            Code = "GRU",
            Name = "São Paulo/Guarulhos International Airport",
            City = "São Paulo",
            Country = "Brazil",
            CountryCode = "BR",
        },
        new Airport
        {
            Id = 5,
            Code = "GIG",
            Name = "Rio de Janeiro/Galeão International Airport",
            City = "Rio de Janeiro",
            Country = "Brazil",
            CountryCode = "BR",
        },
        new Airport
        {
            Id = 6,
            Code = "SCL",
            Name = "Arturo Merino Benítez International Airport",
            City = "Santiago",
            Country = "Chile",
            CountryCode = "CL",
        },
    ];

    public static AirportDto MapAirportToDto(Airport airport)
    {
        if (airport == null) throw new ArgumentNullException(nameof(airport));

        return new AirportDto
        {
            Code = airport.Code,
            Name = airport.Name,
            City = airport.City,
            Country = airport.Country,
            CountryCode = airport.CountryCode
        };
    }

}
