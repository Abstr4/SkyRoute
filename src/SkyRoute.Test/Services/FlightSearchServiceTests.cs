using Moq;
using SkyRoute.Application.Contracts.Requests;
using SkyRoute.Application.DTOs;
using SkyRoute.Application.Interfaces;
using SkyRoute.Application.Services;
using SkyRoute.Domain.Models;

namespace SkyRoute.Test.Services;

[Trait("Category", "Unit")]
public sealed class FlightSearchServiceTests
{
    private readonly Mock<IFlightProvider> _provider1;
    private readonly Mock<IFlightProvider> _provider2;
    private readonly FlightSearchService _service;

    public FlightSearchServiceTests()
    {
        _provider1 = new Mock<IFlightProvider>();
        _provider2 = new Mock<IFlightProvider>();

        _provider1.Setup(p => p.Search(It.IsAny<FlightSearchRequest>())).Returns([]);
        _provider2.Setup(p => p.Search(It.IsAny<FlightSearchRequest>())).Returns([]);

        _service = new FlightSearchService([_provider1.Object, _provider2.Object]);
    }

    [Fact]
    public void Search_MultipleProviders_AggregatesAllOffers()
    {
        var request = CreateValidRequest();
        _provider1.Setup(p => p.Search(request)).Returns([CreateOffer("BW101", "BudgetWings")]);
        _provider2.Setup(p => p.Search(request)).Returns([CreateOffer("GA102", "GlobalAir")]);

        var results = _service.Search(request);

        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.FlightNumber == "BW101");
        Assert.Contains(results, r => r.FlightNumber == "GA102");
    }

    [Fact]
    public void Search_ValidRequest_CalculatesTotalPrice()
    {
        var request = CreateValidRequest(passengers: 3);
        _provider1.Setup(p => p.Search(request)).Returns([CreateOffer("BW101", "BudgetWings", 100m)]);

        var results = _service.Search(request);

        var result = Assert.Single(results);
        Assert.Equal(100m, result.PricePerPassenger);
        Assert.Equal(300m, result.TotalPrice);
    }

    [Fact]
    public void Search_NoProviders_ReturnsEmptyList()
    {
        var emptyService = new FlightSearchService([]);
        var request = CreateValidRequest();

        var results = emptyService.Search(request);

        Assert.Empty(results);
    }

    [Fact]
    public void Search_SomeProviderReturnsEmpty_StillAggregatesOthers()
    {
        var request = CreateValidRequest();
        _provider1.Setup(p => p.Search(request)).Returns([]);
        _provider2.Setup(p => p.Search(request)).Returns([CreateOffer("GA102", "GlobalAir")]);

        var results = _service.Search(request);

        var result = Assert.Single(results);
        Assert.Equal("GA102", result.FlightNumber);
    }

    [Fact]
    public void Search_OfferIncludesDuration_CalculatedCorrectly()
    {
        var request = CreateValidRequest();
        var depart = DateTime.UtcNow.AddDays(1);
        var arrive = depart.AddHours(4).AddMinutes(30);
        var offer = new FlightOffer
        {
            FlightNumber = "BW101",
            Provider = "BudgetWings",
            OriginAirport = new AirportDto
            {
                Code = "EZE", Name = "A", City = "B", Country = "Argentina", CountryCode = "AR",
            },
            DestinationAirport = new AirportDto
            {
                Code = "GRU", Name = "A", City = "B", Country = "Brazil", CountryCode = "BR",
            },
            DepartureTime = depart,
            ArrivalTime = arrive,
            CabinClass = CabinClass.Economy,
            PricePerPassenger = 100m,
        };
        _provider1.Setup(p => p.Search(request)).Returns([offer]);

        var results = _service.Search(request);

        var result = Assert.Single(results);
        Assert.Equal(270, result.DurationMinutes);
    }

    private static FlightSearchRequest CreateValidRequest(int passengers = 1)
    {
        return new FlightSearchRequest(
            "EZE", "GRU",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            passengers,
            CabinClass.Economy);
    }

    private static FlightOffer CreateOffer(string flightNumber, string provider, decimal price = 100m)
    {
        return new FlightOffer
        {
            FlightNumber = flightNumber,
            Provider = provider,
            OriginAirport = new AirportDto
            {
                Code = "EZE", Name = "A", City = "Buenos Aires",
                Country = "Argentina", CountryCode = "AR",
            },
            DestinationAirport = new AirportDto
            {
                Code = "GRU", Name = "A", City = "São Paulo",
                Country = "Brazil", CountryCode = "BR",
            },
            DepartureTime = DateTime.UtcNow.AddDays(1),
            ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(3),
            CabinClass = CabinClass.Economy,
            PricePerPassenger = price,
        };
    }
}
