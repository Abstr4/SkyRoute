using SkyRoute.Domain.Models;

namespace SkyRoute.Infrastructure.Data;

public static class MockDataStore
{
    public static IReadOnlyList<Airport> Airports { get; } = CreateAirports();

    private static IReadOnlyList<Flight> BudgetWingsFlights { get; } = CreateBudgetWingsFlights();

    private static IReadOnlyList<Flight> GlobalAirFlights { get; } = CreateGlobalAirFlights();

    public static IReadOnlyList<Flight> GetAllFlights() =>
        [.. BudgetWingsFlights, .. GlobalAirFlights];

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
        new Airport
        {
            Id = 7,
            Code = "AEP",
            Name = "Aeroparque Jorge Newbery",
            City = "Buenos Aires",
            Country = "Argentina",
            CountryCode = "AR",
        },
        new Airport
        {
            Id = 8,
            Code = "LIM",
            Name = "Jorge Chávez International Airport",
            City = "Lima",
            Country = "Peru",
            CountryCode = "PE",
        },
    ];

    private static IReadOnlyList<Flight> CreateBudgetWingsFlights()
    {
        var a = Airports;
        return
        [
            new Flight
            {
                Id = 1,
                Provider = "BudgetWings",
                FlightNumber = "BW101",
                OriginAirport = a.First(x => x.Code == "EZE"),
                DestinationAirport = a.First(x => x.Code == "COR"),
                DepartureTime = DateTimeOffset.UtcNow.Date.AddHours(6).AddMinutes(40),
                ArrivalTime = DateTimeOffset.UtcNow.Date.AddHours(8).AddMinutes(10),
                CabinClass = CabinClass.Economy,
                BaseFare = 64.40m,
            },
            new Flight
            {
                Id = 2,
                Provider = "BudgetWings",
                FlightNumber = "BW202",
                OriginAirport = a.First(x => x.Code == "EZE"),
                DestinationAirport = a.First(x => x.Code == "MDZ"),
                DepartureTime = DateTimeOffset.UtcNow.Date.AddHours(10).AddMinutes(15),
                ArrivalTime = DateTimeOffset.UtcNow.Date.AddHours(12).AddMinutes(10),
                CabinClass = CabinClass.Economy,
                BaseFare = 72.20m,
            },
            new Flight
            {
                Id = 3,
                Provider = "BudgetWings",
                FlightNumber = "BW310",
                OriginAirport = a.First(x => x.Code == "COR"),
                DestinationAirport = a.First(x => x.Code == "MDZ"),
                DepartureTime = DateTimeOffset.UtcNow.Date.AddHours(7).AddMinutes(0),
                ArrivalTime = DateTimeOffset.UtcNow.Date.AddHours(8).AddMinutes(0),
                CabinClass = CabinClass.Economy,
                BaseFare = 45.00m,
            },
            new Flight
            {
                Id = 4,
                Provider = "BudgetWings",
                FlightNumber = "BW311",
                OriginAirport = a.First(x => x.Code == "COR"),
                DestinationAirport = a.First(x => x.Code == "MDZ"),
                DepartureTime = DateTimeOffset.UtcNow.Date.AddHours(13).AddMinutes(0),
                ArrivalTime = DateTimeOffset.UtcNow.Date.AddHours(14).AddMinutes(0),
                CabinClass = CabinClass.Economy,
                BaseFare = 50.00m,
            },
            new Flight
            {
                Id = 5,
                Provider = "BudgetWings",
                FlightNumber = "BW312",
                OriginAirport = a.First(x => x.Code == "COR"),
                DestinationAirport = a.First(x => x.Code == "MDZ"),
                DepartureTime = DateTimeOffset.UtcNow.Date.AddHours(15).AddMinutes(0),
                ArrivalTime = DateTimeOffset.UtcNow.Date.AddHours(16).AddMinutes(0),
                CabinClass = CabinClass.Economy,
                BaseFare = 55.00m,
            },
            new Flight
            {
                Id = 6,
                Provider = "BudgetWings",
                FlightNumber = "BW313",
                OriginAirport = a.First(x => x.Code == "COR"),
                DestinationAirport = a.First(x => x.Code == "MDZ"),
                DepartureTime = DateTimeOffset.UtcNow.Date.AddHours(19).AddMinutes(0),
                ArrivalTime = DateTimeOffset.UtcNow.Date.AddHours(20).AddMinutes(0),
                CabinClass = CabinClass.Economy,
                BaseFare = 60.00m,
            },
            new Flight
            {
                Id = 7,
                Provider = "BudgetWings",
                FlightNumber = "BW314",
                OriginAirport = a.First(x => x.Code == "COR"),
                DestinationAirport = a.First(x => x.Code == "MDZ"),
                DepartureTime = DateTimeOffset.UtcNow.Date.AddHours(21).AddMinutes(0),
                ArrivalTime = DateTimeOffset.UtcNow.Date.AddHours(22).AddMinutes(0),
                CabinClass = CabinClass.Economy,
                BaseFare = 65.00m,
            },
            new Flight
            {
                Id = 8,
                Provider = "BudgetWings",
                FlightNumber = "BW303",
                OriginAirport = a.First(x => x.Code == "COR"),
                DestinationAirport = a.First(x => x.Code == "MDZ"),
                DepartureTime = DateTimeOffset.UtcNow.AddDays(3).Date.AddHours(14).AddMinutes(30),
                ArrivalTime = DateTimeOffset.UtcNow.AddDays(3).Date.AddHours(15).AddMinutes(45),
                CabinClass = CabinClass.Economy,
                BaseFare = 45.00m,
            },
            new Flight
            {
                Id = 9,
                Provider = "BudgetWings",
                FlightNumber = "BW404",
                OriginAirport = a.First(x => x.Code == "EZE"),
                DestinationAirport = a.First(x => x.Code == "GRU"),
                DepartureTime = DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(7).AddMinutes(0),
                ArrivalTime = DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(10).AddMinutes(15),
                CabinClass = CabinClass.Economy,
                BaseFare = 120.00m,
            },
            new Flight
            {
                Id = 10,
                Provider = "BudgetWings",
                FlightNumber = "BW408",
                OriginAirport = a.First(x => x.Code == "EZE"),
                DestinationAirport = a.First(x => x.Code == "GRU"),
                DepartureTime = DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(15).AddMinutes(0),
                ArrivalTime = DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(18).AddMinutes(15),
                CabinClass = CabinClass.Economy,
                BaseFare = 130.00m,
            },
            new Flight
            {
                Id = 11,
                Provider = "BudgetWings",
                FlightNumber = "BW707",
                OriginAirport = a.First(x => x.Code == "GRU"),
                DestinationAirport = a.First(x => x.Code == "EZE"),
                DepartureTime = DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(8).AddMinutes(0),
                ArrivalTime = DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(11).AddMinutes(30),
                CabinClass = CabinClass.Economy,
                BaseFare = 140.00m,
            },
            new Flight
            {
                Id = 12,
                Provider = "BudgetWings",
                FlightNumber = "BW708",
                OriginAirport = a.First(x => x.Code == "GRU"),
                DestinationAirport = a.First(x => x.Code == "EZE"),
                DepartureTime = DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(16).AddMinutes(0),
                ArrivalTime = DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(19).AddMinutes(30),
                CabinClass = CabinClass.Economy,
                BaseFare = 150.00m,
            },
            new Flight
            {
                Id = 13,
                Provider = "BudgetWings",
                FlightNumber = "BW505",
                OriginAirport = a.First(x => x.Code == "EZE"),
                DestinationAirport = a.First(x => x.Code == "SCL"),
                DepartureTime = DateTimeOffset.UtcNow.AddDays(3).Date.AddHours(18).AddMinutes(10),
                ArrivalTime = DateTimeOffset.UtcNow.AddDays(3).Date.AddHours(20).AddMinutes(35),
                CabinClass = CabinClass.Economy,
                BaseFare = 98.50m,
            },
            new Flight
            {
                Id = 14,
                Provider = "BudgetWings",
                FlightNumber = "BW808",
                OriginAirport = a.First(x => x.Code == "SCL"),
                DestinationAirport = a.First(x => x.Code == "LIM"),
                DepartureTime = DateTimeOffset.UtcNow.AddDays(3).Date.AddHours(9).AddMinutes(0),
                ArrivalTime = DateTimeOffset.UtcNow.AddDays(3).Date.AddHours(11).AddMinutes(0),
                CabinClass = CabinClass.Economy,
                BaseFare = 85.00m,
            },
        ];
    }

    private static IReadOnlyList<Flight> CreateGlobalAirFlights()
    {
        var a = Airports;
        return
        [
            new Flight
            {
                Id = 1,
                Provider = "GlobalAir",
                FlightNumber = "GA102",
                OriginAirport = a.First(x => x.Code == "GRU"),
                DestinationAirport = a.First(x => x.Code == "GIG"),
                DepartureTime = DateTimeOffset.UtcNow.Date.AddHours(6).AddMinutes(0),
                ArrivalTime = DateTimeOffset.UtcNow.Date.AddHours(7).AddMinutes(0),
                CabinClass = CabinClass.Economy,
                BaseFare = 85.00m,
            },
            new Flight
            {
                Id = 2,
                Provider = "GlobalAir",
                FlightNumber = "GA203",
                OriginAirport = a.First(x => x.Code == "GIG"),
                DestinationAirport = a.First(x => x.Code == "GRU"),
                DepartureTime = DateTimeOffset.UtcNow.Date.AddHours(9).AddMinutes(30),
                ArrivalTime = DateTimeOffset.UtcNow.Date.AddHours(10).AddMinutes(30),
                CabinClass = CabinClass.Economy,
                BaseFare = 79.00m,
            },
            new Flight
            {
                Id = 3,
                Provider = "GlobalAir",
                FlightNumber = "GA301",
                OriginAirport = a.First(x => x.Code == "COR"),
                DestinationAirport = a.First(x => x.Code == "MDZ"),
                DepartureTime = DateTimeOffset.UtcNow.Date.AddHours(8).AddMinutes(0),
                ArrivalTime = DateTimeOffset.UtcNow.Date.AddHours(9).AddMinutes(0),
                CabinClass = CabinClass.Economy,
                BaseFare = 50.00m,
            },
            new Flight
            {
                Id = 4,
                Provider = "GlobalAir",
                FlightNumber = "GA302",
                OriginAirport = a.First(x => x.Code == "COR"),
                DestinationAirport = a.First(x => x.Code == "MDZ"),
                DepartureTime = DateTimeOffset.UtcNow.Date.AddHours(9).AddMinutes(30),
                ArrivalTime = DateTimeOffset.UtcNow.Date.AddHours(10).AddMinutes(30),
                CabinClass = CabinClass.Economy,
                BaseFare = 55.00m,
            },
            new Flight
            {
                Id = 5,
                Provider = "GlobalAir",
                FlightNumber = "GA303",
                OriginAirport = a.First(x => x.Code == "COR"),
                DestinationAirport = a.First(x => x.Code == "MDZ"),
                DepartureTime = DateTimeOffset.UtcNow.Date.AddHours(14).AddMinutes(0),
                ArrivalTime = DateTimeOffset.UtcNow.Date.AddHours(15).AddMinutes(0),
                CabinClass = CabinClass.Economy,
                BaseFare = 60.00m,
            },
            new Flight
            {
                Id = 6,
                Provider = "GlobalAir",
                FlightNumber = "GA304",
                OriginAirport = a.First(x => x.Code == "COR"),
                DestinationAirport = a.First(x => x.Code == "MDZ"),
                DepartureTime = DateTimeOffset.UtcNow.Date.AddHours(16).AddMinutes(30),
                ArrivalTime = DateTimeOffset.UtcNow.Date.AddHours(17).AddMinutes(30),
                CabinClass = CabinClass.Economy,
                BaseFare = 65.00m,
            },
            new Flight
            {
                Id = 7,
                Provider = "GlobalAir",
                FlightNumber = "GA307",
                OriginAirport = a.First(x => x.Code == "COR"),
                DestinationAirport = a.First(x => x.Code == "MDZ"),
                DepartureTime = DateTimeOffset.UtcNow.Date.AddHours(20).AddMinutes(0),
                ArrivalTime = DateTimeOffset.UtcNow.Date.AddHours(21).AddMinutes(0),
                CabinClass = CabinClass.Economy,
                BaseFare = 70.00m,
            },
            new Flight
            {
                Id = 8,
                Provider = "GlobalAir",
                FlightNumber = "GA305",
                OriginAirport = a.First(x => x.Code == "GRU"),
                DestinationAirport = a.First(x => x.Code == "EZE"),
                DepartureTime = DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(12).AddMinutes(5),
                ArrivalTime = DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(15).AddMinutes(25),
                CabinClass = CabinClass.Economy,
                BaseFare = 200.00m,
            },
            new Flight
            {
                Id = 9,
                Provider = "GlobalAir",
                FlightNumber = "GA306",
                OriginAirport = a.First(x => x.Code == "GRU"),
                DestinationAirport = a.First(x => x.Code == "EZE"),
                DepartureTime = DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(19).AddMinutes(0),
                ArrivalTime = DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(22).AddMinutes(20),
                CabinClass = CabinClass.Economy,
                BaseFare = 210.00m,
            },
            new Flight
            {
                Id = 10,
                Provider = "GlobalAir",
                FlightNumber = "GA401",
                OriginAirport = a.First(x => x.Code == "EZE"),
                DestinationAirport = a.First(x => x.Code == "GRU"),
                DepartureTime = DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(11).AddMinutes(0),
                ArrivalTime = DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(14).AddMinutes(20),
                CabinClass = CabinClass.Economy,
                BaseFare = 190.00m,
            },
            new Flight
            {
                Id = 11,
                Provider = "GlobalAir",
                FlightNumber = "GA402",
                OriginAirport = a.First(x => x.Code == "EZE"),
                DestinationAirport = a.First(x => x.Code == "GRU"),
                DepartureTime = DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(20).AddMinutes(0),
                ArrivalTime = DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(23).AddMinutes(20),
                CabinClass = CabinClass.Economy,
                BaseFare = 220.00m,
            },
            new Flight
            {
                Id = 12,
                Provider = "GlobalAir",
                FlightNumber = "GA412",
                OriginAirport = a.First(x => x.Code == "GIG"),
                DestinationAirport = a.First(x => x.Code == "SCL"),
                DepartureTime = DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(16).AddMinutes(40),
                ArrivalTime = DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(21).AddMinutes(10),
                CabinClass = CabinClass.Economy,
                BaseFare = 280.00m,
            },
            new Flight
            {
                Id = 13,
                Provider = "GlobalAir",
                FlightNumber = "GA510",
                OriginAirport = a.First(x => x.Code == "EZE"),
                DestinationAirport = a.First(x => x.Code == "SCL"),
                DepartureTime = DateTimeOffset.UtcNow.AddDays(3).Date.AddHours(7).AddMinutes(30),
                ArrivalTime = DateTimeOffset.UtcNow.AddDays(3).Date.AddHours(9).AddMinutes(50),
                CabinClass = CabinClass.Economy,
                BaseFare = 105.00m,
            },
            new Flight
            {
                Id = 14,
                Provider = "GlobalAir",
                FlightNumber = "GA511",
                OriginAirport = a.First(x => x.Code == "EZE"),
                DestinationAirport = a.First(x => x.Code == "SCL"),
                DepartureTime = DateTimeOffset.UtcNow.AddDays(3).Date.AddHours(13).AddMinutes(0),
                ArrivalTime = DateTimeOffset.UtcNow.AddDays(3).Date.AddHours(15).AddMinutes(20),
                CabinClass = CabinClass.Economy,
                BaseFare = 115.00m,
            },
            new Flight
            {
                Id = 15,
                Provider = "GlobalAir",
                FlightNumber = "GA789",
                OriginAirport = a.First(x => x.Code == "SCL"),
                DestinationAirport = a.First(x => x.Code == "LIM"),
                DepartureTime = DateTimeOffset.UtcNow.AddDays(3).Date.AddHours(21).AddMinutes(30),
                ArrivalTime = DateTimeOffset.UtcNow.AddDays(3).Date.AddHours(23).AddMinutes(55),
                CabinClass = CabinClass.Economy,
                BaseFare = 175.00m,
            },
        ];
    }
}
