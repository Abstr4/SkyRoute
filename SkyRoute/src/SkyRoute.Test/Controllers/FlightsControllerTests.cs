using Microsoft.AspNetCore.Mvc;
using Moq;
using SkyRoute.Application.Contracts.Requests;
using SkyRoute.Application.Contracts.Responses;
using SkyRoute.Application.DTOs;
using SkyRoute.Application.Interfaces;
using SkyRoute.API.Controllers;
using SkyRoute.Domain.Models;

namespace SkyRoute.Test.Controllers;

[Trait("Category", "Unit")]
public sealed class FlightsControllerTests
{
    private readonly Mock<IFlightSearchService> _searchServiceMock;
    private readonly FlightsController _controller;

    public FlightsControllerTests()
    {
        _searchServiceMock = new Mock<IFlightSearchService>();
        _controller = new FlightsController(_searchServiceMock.Object);
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
        var request = new FlightSearchRequest(
            "EZE", "GRU",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            1, CabinClass.Economy);
        var expected = new List<FlightSearchResponse>
        {
            new()
            {
                Provider = "BudgetWings", FlightNumber = "BW101",
                OriginAirport = new AirportDto
                {
                    Code = "EZE", Name = "A", City = "B",
                    Country = "Argentina", CountryCode = "AR",
                },
                DestinationAirport = new AirportDto
                {
                    Code = "GRU", Name = "A", City = "B",
                    Country = "Brazil", CountryCode = "BR",
                },
                DepartureTime = DateTime.UtcNow.AddDays(7),
                ArrivalTime = DateTime.UtcNow.AddDays(7).AddHours(3),
                DurationMinutes = 180, CabinClass = CabinClass.Economy,
                PricePerPassenger = 100m, TotalPrice = 100m,
            },
        };
        _searchServiceMock.Setup(s => s.Search(request)).Returns(expected);

        var result = _controller.SearchFlights(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var results = Assert.IsAssignableFrom<IReadOnlyList<FlightSearchResponse>>(okResult.Value);
        Assert.Single(results);
    }

    [Fact]
    public void SearchFlights_ServiceThrowsException_Returns500()
    {
        var request = new FlightSearchRequest(
            "EZE", "GRU",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            1, CabinClass.Economy);
        _searchServiceMock.Setup(s => s.Search(request))
            .Throws(new Exception("unexpected"));

        var result = _controller.SearchFlights(request);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }
}
