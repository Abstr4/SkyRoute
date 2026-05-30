using SkyRoute.Domain.Models;

namespace SkyRoute.Infrastructure.Data;

public static class MockDataStore
{
    public static IReadOnlyList<Airport> Airports { get; } = CreateAirports();

    public static readonly List<Booking> Bookings = new();

    public static readonly string[] CabinClasses = ["Economy", "Business", "FirstClass"];

    public static IReadOnlyList<Flight> BudgetWingsFlights { get; } = CreateBudgetWingsFlights();

    public static IReadOnlyList<Flight> GlobalAirFlights { get; } = CreateGlobalAirFlights();

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
                DepartureTime = DateTime.UtcNow.Date.AddHours(6).AddMinutes(40),
                ArrivalTime = DateTime.UtcNow.Date.AddHours(8).AddMinutes(10),
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
                DepartureTime = DateTime.UtcNow.Date.AddHours(10).AddMinutes(15),
                ArrivalTime = DateTime.UtcNow.Date.AddHours(12).AddMinutes(10),
                CabinClass = CabinClass.Economy,
                BaseFare = 72.20m,
            },
            new Flight
            {
                Id = 3,
                Provider = "BudgetWings",
                FlightNumber = "BW303",
                OriginAirport = a.First(x => x.Code == "COR"),
                DestinationAirport = a.First(x => x.Code == "MDZ"),
                DepartureTime = DateTime.UtcNow.Date.AddHours(14).AddMinutes(30),
                ArrivalTime = DateTime.UtcNow.Date.AddHours(15).AddMinutes(45),
                CabinClass = CabinClass.Economy,
                BaseFare = 45.00m,
            },
            new Flight
            {
                Id = 4,
                Provider = "BudgetWings",
                FlightNumber = "BW404",
                OriginAirport = a.First(x => x.Code == "EZE"),
                DestinationAirport = a.First(x => x.Code == "GRU"),
                DepartureTime = DateTime.UtcNow.Date.AddHours(7).AddMinutes(0),
                ArrivalTime = DateTime.UtcNow.Date.AddHours(10).AddMinutes(15),
                CabinClass = CabinClass.Economy,
                BaseFare = 120.00m,
            },
            new Flight
            {
                Id = 5,
                Provider = "BudgetWings",
                FlightNumber = "BW505",
                OriginAirport = a.First(x => x.Code == "EZE"),
                DestinationAirport = a.First(x => x.Code == "SCL"),
                DepartureTime = DateTime.UtcNow.Date.AddHours(18).AddMinutes(10),
                ArrivalTime = DateTime.UtcNow.Date.AddHours(20).AddMinutes(35),
                CabinClass = CabinClass.Economy,
                BaseFare = 98.50m,
            },
            new Flight
            {
                Id = 6,
                Provider = "BudgetWings",
                FlightNumber = "BW606",
                OriginAirport = a.First(x => x.Code == "COR"),
                DestinationAirport = a.First(x => x.Code == "GRU"),
                DepartureTime = DateTime.UtcNow.Date.AddHours(20).AddMinutes(50),
                ArrivalTime = DateTime.UtcNow.Date.AddHours(23).AddMinutes(59),
                CabinClass = CabinClass.Economy,
                BaseFare = 110.00m,
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
                DepartureTime = DateTime.UtcNow.Date.AddHours(6).AddMinutes(0),
                ArrivalTime = DateTime.UtcNow.Date.AddHours(7).AddMinutes(0),
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
                DepartureTime = DateTime.UtcNow.Date.AddHours(9).AddMinutes(30),
                ArrivalTime = DateTime.UtcNow.Date.AddHours(10).AddMinutes(30),
                CabinClass = CabinClass.Economy,
                BaseFare = 79.00m,
            },
            new Flight
            {
                Id = 3,
                Provider = "GlobalAir",
                FlightNumber = "GA305",
                OriginAirport = a.First(x => x.Code == "GRU"),
                DestinationAirport = a.First(x => x.Code == "EZE"),
                DepartureTime = DateTime.UtcNow.Date.AddHours(12).AddMinutes(5),
                ArrivalTime = DateTime.UtcNow.Date.AddHours(15).AddMinutes(25),
                CabinClass = CabinClass.Economy,
                BaseFare = 200.00m,
            },
            new Flight
            {
                Id = 4,
                Provider = "GlobalAir",
                FlightNumber = "GA412",
                OriginAirport = a.First(x => x.Code == "GIG"),
                DestinationAirport = a.First(x => x.Code == "SCL"),
                DepartureTime = DateTime.UtcNow.Date.AddHours(16).AddMinutes(40),
                ArrivalTime = DateTime.UtcNow.Date.AddHours(21).AddMinutes(10),
                CabinClass = CabinClass.Economy,
                BaseFare = 280.00m,
            },
            new Flight
            {
                Id = 5,
                Provider = "GlobalAir",
                FlightNumber = "GA550",
                OriginAirport = a.First(x => x.Code == "GRU"),
                DestinationAirport = a.First(x => x.Code == "SCL"),
                DepartureTime = DateTime.UtcNow.Date.AddHours(14).AddMinutes(0),
                ArrivalTime = DateTime.UtcNow.Date.AddHours(18).AddMinutes(45),
                CabinClass = CabinClass.Economy,
                BaseFare = 260.00m,
            },
            new Flight
            {
                Id = 6,
                Provider = "GlobalAir",
                FlightNumber = "GA789",
                OriginAirport = a.First(x => x.Code == "SCL"),
                DestinationAirport = a.First(x => x.Code == "LIM"),
                DepartureTime = DateTime.UtcNow.Date.AddHours(21).AddMinutes(30),
                ArrivalTime = DateTime.UtcNow.Date.AddHours(23).AddMinutes(55),
                CabinClass = CabinClass.Economy,
                BaseFare = 175.00m,
            },
            new Flight
            {
                Id = 7,
                Provider = "GlobalAir",
                FlightNumber = "GA888",
                OriginAirport = a.First(x => x.Code == "GIG"),
                DestinationAirport = a.First(x => x.Code == "LIM"),
                DepartureTime = DateTime.UtcNow.Date.AddHours(22).AddMinutes(15),
                ArrivalTime = DateTime.UtcNow.Date.AddHours(3).AddMinutes(15).AddDays(1),
                CabinClass = CabinClass.Economy,
                BaseFare = 220.00m,
            },
            new Flight
            {
                Id = 8,
                Provider = "GlobalAir",
                FlightNumber = "GA900",
                OriginAirport = a.First(x => x.Code == "COR"),
                DestinationAirport = a.First(x => x.Code == "MDZ"),
                DepartureTime = DateTime.UtcNow.Date.AddHours(15).AddMinutes(30),
                ArrivalTime = DateTime.UtcNow.Date.AddHours(16).AddMinutes(45),
                CabinClass = CabinClass.Economy,
                BaseFare = 45.00m,
            },
            new Flight
            {
                Id = 9,
                Provider = "GlobalAir",
                FlightNumber = "GA901",
                OriginAirport = a.First(x => x.Code == "COR"),
                DestinationAirport = a.First(x => x.Code == "MDZ"),
                DepartureTime = DateTime.UtcNow.Date.AddHours(15).AddMinutes(30),
                ArrivalTime = DateTime.UtcNow.Date.AddHours(16).AddMinutes(45),
                CabinClass = CabinClass.Economy,
                BaseFare = 45.00m,
            },
            new Flight
            {
                Id = 10,
                Provider = "GlobalAir",
                FlightNumber = "GA902",
                OriginAirport = a.First(x => x.Code == "COR"),
                DestinationAirport = a.First(x => x.Code == "MDZ"),
                DepartureTime = DateTime.UtcNow.Date.AddHours(15).AddMinutes(30),
                ArrivalTime = DateTime.UtcNow.Date.AddHours(16).AddMinutes(45),
                CabinClass = CabinClass.Economy,
                BaseFare = 45.00m,
            },
        ];
    }
}
