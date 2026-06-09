using SkyRoute.Domain.Models;

namespace SkyRoute.Infrastructure.Data;

public static class SeedData
{
    public static List<Airport> CreateAirports() =>
    [
        new Airport { Id = 1, Code = "EZE", Name = "Ministro Pistarini International Airport", City = "Buenos Aires", Country = "Argentina", CountryCode = "AR" },
        new Airport { Id = 2, Code = "COR", Name = "Ingeniero Aeronáutico Ambrosio L.V. Taravella International Airport", City = "Córdoba", Country = "Argentina", CountryCode = "AR" },
        new Airport { Id = 3, Code = "MDZ", Name = "Governor Francisco Gabrielli International Airport", City = "Mendoza", Country = "Argentina", CountryCode = "AR" },
        new Airport { Id = 4, Code = "GRU", Name = "São Paulo/Guarulhos International Airport", City = "São Paulo", Country = "Brazil", CountryCode = "BR" },
        new Airport { Id = 5, Code = "GIG", Name = "Rio de Janeiro/Galeão International Airport", City = "Rio de Janeiro", Country = "Brazil", CountryCode = "BR" },
        new Airport { Id = 6, Code = "SCL", Name = "Arturo Merino Benítez International Airport", City = "Santiago", Country = "Chile", CountryCode = "CL" },
        new Airport { Id = 7, Code = "AEP", Name = "Aeroparque Jorge Newbery", City = "Buenos Aires", Country = "Argentina", CountryCode = "AR" },
        new Airport { Id = 8, Code = "LIM", Name = "Jorge Chávez International Airport", City = "Lima", Country = "Peru", CountryCode = "PE" },
    ];

    public static List<Flight> CreateFlights(List<Airport> airports)
    {
        var a = airports.ToDictionary(x => x.Code);
        var today = DateTimeOffset.UtcNow.Date;
        var offset = TimeSpan.Zero;

        Flight F(int id, string provider, string number, string origin, string dest, DateTimeOffset dep, DateTimeOffset arr, CabinClass cabin, decimal fare) => new()
        {
            Id = id, Provider = provider, FlightNumber = number,
            OriginAirportId = a[origin].Id, OriginAirport = a[origin],
            DestinationAirportId = a[dest].Id, DestinationAirport = a[dest],
            DepartureTime = dep, ArrivalTime = arr,
            CabinClass = cabin, BaseFare = fare,
        };

        return
        [
            // BudgetWings (IDs 1-14)
            F(1, "BudgetWings", "BW101", "EZE", "COR", new DateTimeOffset(today, offset).AddHours(6).AddMinutes(40), new DateTimeOffset(today, offset).AddHours(8).AddMinutes(10), CabinClass.Economy, 64.40m),
            F(2, "BudgetWings", "BW202", "EZE", "MDZ", new DateTimeOffset(today, offset).AddHours(10).AddMinutes(15), new DateTimeOffset(today, offset).AddHours(12).AddMinutes(10), CabinClass.Economy, 72.20m),
            F(3, "BudgetWings", "BW310", "COR", "MDZ", new DateTimeOffset(today, offset).AddHours(7).AddMinutes(0), new DateTimeOffset(today, offset).AddHours(8).AddMinutes(0), CabinClass.Economy, 45.00m),
            F(4, "BudgetWings", "BW311", "COR", "MDZ", new DateTimeOffset(today, offset).AddHours(13).AddMinutes(0), new DateTimeOffset(today, offset).AddHours(14).AddMinutes(0), CabinClass.Economy, 50.00m),
            F(5, "BudgetWings", "BW312", "COR", "MDZ", new DateTimeOffset(today, offset).AddHours(15).AddMinutes(0), new DateTimeOffset(today, offset).AddHours(16).AddMinutes(0), CabinClass.Economy, 55.00m),
            F(6, "BudgetWings", "BW313", "COR", "MDZ", new DateTimeOffset(today, offset).AddHours(19).AddMinutes(0), new DateTimeOffset(today, offset).AddHours(20).AddMinutes(0), CabinClass.Economy, 60.00m),
            F(7, "BudgetWings", "BW314", "COR", "MDZ", new DateTimeOffset(today, offset).AddHours(21).AddMinutes(0), new DateTimeOffset(today, offset).AddHours(22).AddMinutes(0), CabinClass.Economy, 65.00m),
            F(8, "BudgetWings", "BW303", "COR", "MDZ", new DateTimeOffset(today.AddDays(3), offset).AddHours(14).AddMinutes(30), new DateTimeOffset(today.AddDays(3), offset).AddHours(15).AddMinutes(45), CabinClass.Economy, 45.00m),
            F(9, "BudgetWings", "BW404", "EZE", "GRU", new DateTimeOffset(today.AddDays(1), offset).AddHours(7).AddMinutes(0), new DateTimeOffset(today.AddDays(1), offset).AddHours(10).AddMinutes(15), CabinClass.Economy, 120.00m),
            F(10, "BudgetWings", "BW408", "EZE", "GRU", new DateTimeOffset(today.AddDays(1), offset).AddHours(15).AddMinutes(0), new DateTimeOffset(today.AddDays(1), offset).AddHours(18).AddMinutes(15), CabinClass.Economy, 130.00m),
            F(11, "BudgetWings", "BW707", "GRU", "EZE", new DateTimeOffset(today.AddDays(1), offset).AddHours(8).AddMinutes(0), new DateTimeOffset(today.AddDays(1), offset).AddHours(11).AddMinutes(30), CabinClass.Economy, 140.00m),
            F(12, "BudgetWings", "BW708", "GRU", "EZE", new DateTimeOffset(today.AddDays(1), offset).AddHours(16).AddMinutes(0), new DateTimeOffset(today.AddDays(1), offset).AddHours(19).AddMinutes(30), CabinClass.Economy, 150.00m),
            F(13, "BudgetWings", "BW505", "EZE", "SCL", new DateTimeOffset(today.AddDays(3), offset).AddHours(18).AddMinutes(10), new DateTimeOffset(today.AddDays(3), offset).AddHours(20).AddMinutes(35), CabinClass.Economy, 98.50m),
            F(14, "BudgetWings", "BW808", "SCL", "LIM", new DateTimeOffset(today.AddDays(3), offset).AddHours(9).AddMinutes(0), new DateTimeOffset(today.AddDays(3), offset).AddHours(11).AddMinutes(0), CabinClass.Economy, 85.00m),

            // GlobalAir (IDs 15-29)
            F(15, "GlobalAir", "GA102", "GRU", "GIG", new DateTimeOffset(today, offset).AddHours(6).AddMinutes(0), new DateTimeOffset(today, offset).AddHours(7).AddMinutes(0), CabinClass.Economy, 85.00m),
            F(16, "GlobalAir", "GA203", "GIG", "GRU", new DateTimeOffset(today, offset).AddHours(9).AddMinutes(30), new DateTimeOffset(today, offset).AddHours(10).AddMinutes(30), CabinClass.Economy, 79.00m),
            F(17, "GlobalAir", "GA301", "COR", "MDZ", new DateTimeOffset(today, offset).AddHours(8).AddMinutes(0), new DateTimeOffset(today, offset).AddHours(9).AddMinutes(0), CabinClass.Economy, 50.00m),
            F(18, "GlobalAir", "GA302", "COR", "MDZ", new DateTimeOffset(today, offset).AddHours(9).AddMinutes(30), new DateTimeOffset(today, offset).AddHours(10).AddMinutes(30), CabinClass.Economy, 55.00m),
            F(19, "GlobalAir", "GA303", "COR", "MDZ", new DateTimeOffset(today, offset).AddHours(14).AddMinutes(0), new DateTimeOffset(today, offset).AddHours(15).AddMinutes(0), CabinClass.Economy, 60.00m),
            F(20, "GlobalAir", "GA304", "COR", "MDZ", new DateTimeOffset(today, offset).AddHours(16).AddMinutes(30), new DateTimeOffset(today, offset).AddHours(17).AddMinutes(30), CabinClass.Economy, 65.00m),
            F(21, "GlobalAir", "GA307", "COR", "MDZ", new DateTimeOffset(today, offset).AddHours(20).AddMinutes(0), new DateTimeOffset(today, offset).AddHours(21).AddMinutes(0), CabinClass.Economy, 70.00m),
            F(22, "GlobalAir", "GA305", "GRU", "EZE", new DateTimeOffset(today.AddDays(1), offset).AddHours(12).AddMinutes(5), new DateTimeOffset(today.AddDays(1), offset).AddHours(15).AddMinutes(25), CabinClass.Economy, 200.00m),
            F(23, "GlobalAir", "GA306", "GRU", "EZE", new DateTimeOffset(today.AddDays(1), offset).AddHours(19).AddMinutes(0), new DateTimeOffset(today.AddDays(1), offset).AddHours(22).AddMinutes(20), CabinClass.Economy, 210.00m),
            F(24, "GlobalAir", "GA401", "EZE", "GRU", new DateTimeOffset(today.AddDays(1), offset).AddHours(11).AddMinutes(0), new DateTimeOffset(today.AddDays(1), offset).AddHours(14).AddMinutes(20), CabinClass.Economy, 190.00m),
            F(25, "GlobalAir", "GA402", "EZE", "GRU", new DateTimeOffset(today.AddDays(1), offset).AddHours(20).AddMinutes(0), new DateTimeOffset(today.AddDays(1), offset).AddHours(23).AddMinutes(20), CabinClass.Economy, 220.00m),
            F(26, "GlobalAir", "GA412", "GIG", "SCL", new DateTimeOffset(today.AddDays(1), offset).AddHours(16).AddMinutes(40), new DateTimeOffset(today.AddDays(1), offset).AddHours(21).AddMinutes(10), CabinClass.Economy, 280.00m),
            F(27, "GlobalAir", "GA510", "EZE", "SCL", new DateTimeOffset(today.AddDays(3), offset).AddHours(7).AddMinutes(30), new DateTimeOffset(today.AddDays(3), offset).AddHours(9).AddMinutes(50), CabinClass.Economy, 105.00m),
            F(28, "GlobalAir", "GA511", "EZE", "SCL", new DateTimeOffset(today.AddDays(3), offset).AddHours(13).AddMinutes(0), new DateTimeOffset(today.AddDays(3), offset).AddHours(15).AddMinutes(20), CabinClass.Economy, 115.00m),
            F(29, "GlobalAir", "GA789", "SCL", "LIM", new DateTimeOffset(today.AddDays(3), offset).AddHours(21).AddMinutes(30), new DateTimeOffset(today.AddDays(3), offset).AddHours(23).AddMinutes(55), CabinClass.Economy, 175.00m),

            // SuperCheapest (IDs 30-34)
            F(30, "SuperCheapest", "SC101", "EZE", "COR", new DateTimeOffset(today, offset).AddHours(6).AddMinutes(40), new DateTimeOffset(today, offset).AddHours(8).AddMinutes(10), CabinClass.Economy, 64.40m),
            F(31, "SuperCheapest", "SC202", "EZE", "GRU", new DateTimeOffset(today.AddDays(1), offset).AddHours(7).AddMinutes(0), new DateTimeOffset(today.AddDays(1), offset).AddHours(10).AddMinutes(15), CabinClass.Economy, 120.00m),
            F(32, "SuperCheapest", "SC303", "GRU", "GIG", new DateTimeOffset(today, offset).AddHours(6).AddMinutes(0), new DateTimeOffset(today, offset).AddHours(7).AddMinutes(0), CabinClass.Economy, 85.00m),
            F(33, "SuperCheapest", "SC404", "COR", "MDZ", new DateTimeOffset(today, offset).AddHours(8).AddMinutes(0), new DateTimeOffset(today, offset).AddHours(9).AddMinutes(0), CabinClass.Economy, 50.00m),
            F(34, "SuperCheapest", "SC505", "EZE", "SCL", new DateTimeOffset(today.AddDays(3), offset).AddHours(18).AddMinutes(10), new DateTimeOffset(today.AddDays(3), offset).AddHours(20).AddMinutes(35), CabinClass.Economy, 98.50m),
        ];
    }
}
