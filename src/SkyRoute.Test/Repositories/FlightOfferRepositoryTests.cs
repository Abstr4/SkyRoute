using SkyRoute.API.DTOs;
using SkyRoute.API.Models;
using SkyRoute.API.Services;

namespace SkyRoute.Test.Repositories;

[Trait("Category", "Unit")]
public sealed class FlightOfferRepositoryTests
{
    private readonly FlightOfferRepository _repository;

    public FlightOfferRepositoryTests()
    {
        _repository = new FlightOfferRepository();
    }

    [Fact]
    public void StoreOffer_ValidOffer_CanRetrieveById()
    {
        var offer = CreateOffer(1, "BW101", "BudgetWings");

        _repository.StoreOffer(offer);
        var retrieved = _repository.GetOfferById(1);

        Assert.NotNull(retrieved);
        Assert.Equal(1, retrieved.Id);
        Assert.Equal("BW101", retrieved.FlightNumber);
        Assert.Equal("BudgetWings", retrieved.Provider);
    }

    [Fact]
    public void StoreOffers_MultipleOffers_AllRetrievable()
    {
        var offers = new List<FlightOffer>
        {
            CreateOffer(1, "BW101", "BudgetWings"),
            CreateOffer(2, "GA102", "GlobalAir"),
            CreateOffer(3, "BW202", "BudgetWings"),
        };

        _repository.StoreOffers(offers);

        Assert.NotNull(_repository.GetOfferById(1));
        Assert.NotNull(_repository.GetOfferById(2));
        Assert.NotNull(_repository.GetOfferById(3));
    }

    [Fact]
    public void GetOfferById_NonExistentId_ReturnsNull()
    {
        var result = _repository.GetOfferById(999);
        Assert.Null(result);
    }

    [Fact]
    public void StoreOffer_DuplicateId_OverwritesExisting()
    {
        var offer1 = CreateOffer(1, "BW101", "BudgetWings");
        var offer2 = CreateOffer(1, "GA102", "GlobalAir");

        _repository.StoreOffer(offer1);
        _repository.StoreOffer(offer2);
        var retrieved = _repository.GetOfferById(1);

        Assert.NotNull(retrieved);
        Assert.Equal("GA102", retrieved.FlightNumber);
        Assert.Equal("GlobalAir", retrieved.Provider);
    }

    [Fact]
    public void ClearOffers_AfterStoring_EmptiesRepository()
    {
        _repository.StoreOffer(CreateOffer(1, "BW101", "BudgetWings"));
        _repository.StoreOffer(CreateOffer(2, "GA102", "GlobalAir"));

        _repository.ClearOffers();

        Assert.Null(_repository.GetOfferById(1));
        Assert.Null(_repository.GetOfferById(2));
    }

    private static FlightOffer CreateOffer(int id, string flightNumber, string provider)
    {
        return new FlightOffer
        {
            Id = id,
            FlightNumber = flightNumber,
            Provider = provider,
            OriginAirport = new AirportDto
            {
                Code = "EZE",
                Name = "Ministro Pistarini International Airport",
                City = "Buenos Aires",
                Country = "Argentina",
                CountryCode = "AR",
            },
            DestinationAirport = new AirportDto
            {
                Code = "GRU",
                Name = "São Paulo/Guarulhos International Airport",
                City = "São Paulo",
                Country = "Brazil",
                CountryCode = "BR",
            },
            DepartureTime = DateTime.UtcNow.AddDays(1),
            ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(3),
            CabinClass = CabinClass.Economy,
            PricePerPassenger = 100m,
        };
    }
}
