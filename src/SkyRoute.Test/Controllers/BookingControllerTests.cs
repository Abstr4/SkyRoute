using Microsoft.AspNetCore.Mvc;
using SkyRoute.API.Contracts.Requests;
using SkyRoute.API.Contracts.Responses;
using SkyRoute.API.Controllers;
using SkyRoute.API.Data;
using SkyRoute.API.DTOs;
using SkyRoute.API.Models;
using SkyRoute.API.Services;

namespace SkyRoute.Test.Controllers;

[Trait("Category", "Unit")]
public sealed class BookingControllerTests
{
    private readonly FlightOfferRepository _repository;
    private readonly BookingController _controller;

    public BookingControllerTests()
    {
        _repository = new FlightOfferRepository();
        var bookingService = new BookingService(_repository);
        _controller = new BookingController(bookingService);
    }

    [Fact]
    public void CreateBooking_ValidDomesticRequest_Returns201WithReferenceCode()
    {
        var offer = CreateOffer(1, "EZE", "COR", CabinClass.Economy);
        _repository.StoreOffer(offer);
        var request = new CreateBookingRequest(1, new List<CreatePassengerRequest>
        {
            new("John Doe", "john@test.com", DocumentType.NationalId, "12345678"),
        });

        var result = _controller.CreateBooking(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
        var response = Assert.IsType<CreateBookingResponse>(createdResult.Value);
        Assert.StartsWith("SKY-", response.BookingReferenceCode);
    }

    private static string ExtractErrorMessage(IActionResult result)
    {
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var errorProperty = badRequest.Value!.GetType().GetProperty("error");
        Assert.NotNull(errorProperty);
        return (errorProperty.GetValue(badRequest.Value) as string)!;
    }

    [Fact]
    public void CreateBooking_FlightNotFound_Returns400WithError()
    {
        var request = new CreateBookingRequest(999, []);

        var result = _controller.CreateBooking(request);

        var error = ExtractErrorMessage(result);
        Assert.Contains("999", error);
    }

    [Fact]
    public void CreateBooking_InternationalRouteWithNationalId_Returns400WithError()
    {
        var offer = CreateOffer(1, "EZE", "GRU", CabinClass.Economy);
        _repository.StoreOffer(offer);
        var request = new CreateBookingRequest(1, new List<CreatePassengerRequest>
        {
            new("Jane Doe", "jane@test.com", DocumentType.NationalId, "12345678"),
        });

        var result = _controller.CreateBooking(request);

        var error = ExtractErrorMessage(result);
        Assert.Contains("Passport", error);
    }

    private static FlightOffer CreateOffer(int id, string originCode, string destCode, CabinClass cabin)
    {
        var origin = MockDataStore.Airports.First(a => a.Code == originCode);
        var dest = MockDataStore.Airports.First(a => a.Code == destCode);

        return new FlightOffer
        {
            Id = id,
            FlightNumber = "BW101",
            Provider = "BudgetWings",
            OriginAirport = new AirportDto
            {
                Code = origin.Code, Name = origin.Name, City = origin.City,
                Country = origin.Country, CountryCode = origin.CountryCode,
            },
            DestinationAirport = new AirportDto
            {
                Code = dest.Code, Name = dest.Name, City = dest.City,
                Country = dest.Country, CountryCode = dest.CountryCode,
            },
            DepartureTime = DateTime.UtcNow.AddDays(1),
            ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(3),
            CabinClass = cabin,
            PricePerPassenger = 100m,
        };
    }
}
