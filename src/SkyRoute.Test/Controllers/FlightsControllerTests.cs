using Microsoft.AspNetCore.Mvc;
using Moq;
using SkyRoute.API.Contracts.Requests;
using SkyRoute.API.Contracts.Responses;
using SkyRoute.API.Controllers;
using SkyRoute.API.DTOs;
using SkyRoute.API.Models;
using SkyRoute.API.Providers;
using SkyRoute.API.Services;

namespace SkyRoute.Test.Controllers;

[Trait("Category", "Unit")]
public sealed class FlightsControllerTests
{
    private readonly Mock<IFlightProvider> _providerMock;
    private readonly FlightsController _controller;

    public FlightsControllerTests()
    {
        _providerMock = new Mock<IFlightProvider>();
        _providerMock.Setup(p => p.Search(It.IsAny<FlightSearchRequest>())).Returns([]);

        var repository = new FlightOfferRepository();
        var searchService = new FlightSearchService([_providerMock.Object], repository);
        _controller = new FlightsController(searchService);
    }

    [Fact]
    public void SearchFlights_PastDepartureDate_ReturnsBadRequest()
    {
        var request = new FlightSearchRequest(
            "EZE", "GRU",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            1, CabinClass.Economy);

        var result = _controller.SearchFlights(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Departure date cannot be in the past.", badRequest.Value);
    }

    [Fact]
    public void SearchFlights_SameOriginAndDestination_ReturnsBadRequest()
    {
        var request = new FlightSearchRequest(
            "EZE", "EZE",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            1, CabinClass.Economy);

        var result = _controller.SearchFlights(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Origin and destination airports cannot be the same.", badRequest.Value);
    }

    [Fact]
    public void SearchFlights_ValidRequest_ReturnsOkWithResults()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        var request = new FlightSearchRequest("EZE", "GRU", today, 1, CabinClass.Economy);
        var offer = CreateOffer(today);
        _providerMock.Setup(p => p.Search(request)).Returns([offer]);

        var result = _controller.SearchFlights(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var results = Assert.IsAssignableFrom<IReadOnlyList<FlightSearchResponse>>(okResult.Value);
        var single = Assert.Single(results);
        Assert.Equal("BW101", single.FlightNumber);
    }

    [Fact]
    public void SearchFlights_ServiceThrowsException_Returns500()
    {
        var request = new FlightSearchRequest(
            "EZE", "GRU",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            1, CabinClass.Economy);
        _providerMock.Setup(p => p.Search(request)).Throws(new InvalidOperationException("provider error"));

        var result = _controller.SearchFlights(request);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    private static FlightOffer CreateOffer(DateOnly date)
    {
        var baseDate = date.ToDateTime(new TimeOnly(0, 0), DateTimeKind.Utc);
        return new FlightOffer
        {
            Id = 1,
            FlightNumber = "BW101",
            Provider = "BudgetWings",
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
            DepartureTime = baseDate.AddHours(6).AddMinutes(40),
            ArrivalTime = baseDate.AddHours(9).AddMinutes(55),
            CabinClass = CabinClass.Economy,
            PricePerPassenger = 100m,
        };
    }
}
