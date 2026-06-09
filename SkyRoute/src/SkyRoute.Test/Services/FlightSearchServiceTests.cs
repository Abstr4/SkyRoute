using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using SkyRoute.Application.DTOs;
using SkyRoute.Application.Features.Flights;
using SkyRoute.Application.Interfaces;
using SkyRoute.Application.Services;
using SkyRoute.Domain.Models;

namespace SkyRoute.Test.Services;

[Trait("Category", "Unit")]
public sealed class FlightSearchServiceTests
{
    private readonly Mock<IFlightProvider> _provider1;
    private readonly Mock<IFlightProvider> _provider2;
    private readonly Mock<IValidator<FlightSearchRequest>> _validatorMock;
    private readonly FlightSearchService _service;

    public FlightSearchServiceTests()
    {
        _provider1 = new Mock<IFlightProvider>();
        _provider2 = new Mock<IFlightProvider>();
        _validatorMock = new Mock<IValidator<FlightSearchRequest>>();

        _provider1.Setup(p => p.SearchAsync(It.IsAny<FlightSearchRequest>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).ReturnsAsync([]);
        _provider2.Setup(p => p.SearchAsync(It.IsAny<FlightSearchRequest>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).ReturnsAsync([]);
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<FlightSearchRequest>())).ReturnsAsync(new ValidationResult());

        _service = new FlightSearchService(
            [_provider1.Object, _provider2.Object],
            _validatorMock.Object,
            Mock.Of<ILogger<FlightSearchService>>());
    }

    [Fact]
    public async Task Search_MultipleProviders_AggregatesAllOffers()
    {
        var request = CreateValidRequest();
        _provider1.Setup(p => p.SearchAsync(request, It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).ReturnsAsync([CreateOffer("BW101", "BudgetWings")]);
        _provider2.Setup(p => p.SearchAsync(request, It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).ReturnsAsync([CreateOffer("GA102", "GlobalAir")]);

        var result = await _service.SearchAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count);
        Assert.Contains(result.Value, r => r.FlightNumber == "BW101");
        Assert.Contains(result.Value, r => r.FlightNumber == "GA102");
    }

    [Fact]
    public async Task Search_ValidRequest_CalculatesTotalPrice()
    {
        var request = CreateValidRequest(passengers: 3);
        _provider1.Setup(p => p.SearchAsync(request, It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).ReturnsAsync([CreateOffer("BW101", "BudgetWings", 100m)]);

        var result = await _service.SearchAsync(request);

        Assert.True(result.IsSuccess);
        var value = Assert.Single(result.Value!);
        Assert.Equal(100m, value.PricePerPassenger);
        Assert.Equal(300m, value.TotalPrice);
    }

    [Fact]
    public async Task Search_NoProviders_ReturnsEmptyList()
    {
        var passValidator = new Mock<IValidator<FlightSearchRequest>>();
        passValidator.Setup(v => v.ValidateAsync(It.IsAny<FlightSearchRequest>())).ReturnsAsync(new ValidationResult());
        var emptyService = new FlightSearchService([], passValidator.Object, Mock.Of<ILogger<FlightSearchService>>());
        var request = CreateValidRequest();

        var result = await emptyService.SearchAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }

    [Fact]
    public async Task Search_SomeProviderReturnsEmpty_StillAggregatesOthers()
    {
        var request = CreateValidRequest();
        _provider1.Setup(p => p.SearchAsync(request, It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).ReturnsAsync([]);
        _provider2.Setup(p => p.SearchAsync(request, It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).ReturnsAsync([CreateOffer("GA102", "GlobalAir")]);

        var result = await _service.SearchAsync(request);

        Assert.True(result.IsSuccess);
        var value = Assert.Single(result.Value!);
        Assert.Equal("GA102", value.FlightNumber);
    }

    [Fact]
    public async Task Search_OfferIncludesDuration_CalculatedCorrectly()
    {
        var request = CreateValidRequest();
        var depart = DateTimeOffset.UtcNow.AddDays(1);
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
        _provider1.Setup(p => p.SearchAsync(request, It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).ReturnsAsync([offer]);

        var result = await _service.SearchAsync(request);

        Assert.True(result.IsSuccess);
        var value = Assert.Single(result.Value!);
        Assert.Equal(270, value.DurationMinutes);
    }

    private static FlightSearchRequest CreateValidRequest(int passengers = 1)
    {
        return new FlightSearchRequest(
            "EZE", "GRU",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            passengers,
            CabinClass.Economy, "UTC");
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
            DepartureTime = DateTimeOffset.UtcNow.AddDays(1),
            ArrivalTime = DateTimeOffset.UtcNow.AddDays(1).AddHours(3),
            CabinClass = CabinClass.Economy,
            PricePerPassenger = price,
        };
    }
}
