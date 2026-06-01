using Microsoft.AspNetCore.Mvc;
using Moq;
using SkyRoute.Application.Interfaces;
using SkyRoute.API.Controllers;
using SkyRoute.Domain.Models;
using SkyRoute.Application.Features.Booking;

namespace SkyRoute.Test.Controllers;

[Trait("Category", "Unit")]
public sealed class BookingControllerTests
{
    private readonly Mock<IBookingService> _bookingServiceMock;
    private readonly BookingController _controller;

    public BookingControllerTests()
    {
        _bookingServiceMock = new Mock<IBookingService>();
        _controller = new BookingController(_bookingServiceMock.Object);
    }

    [Fact]
    public void CreateBooking_ValidDomesticRequest_Returns201WithReferenceCode()
    {
        var request = new CreateBookingRequest("BudgetWings", "BW101", new List<CreatePassengerRequest>
        {
            new("John Doe", "john@test.com", DocumentType.NationalId, "12345678"),
        });
        var booking = new Booking
        {
            Id = 1,
            ReferenceCode = "SKY-ABC123",
            CreatedAtUtc = DateTime.UtcNow,
            ProviderName = "BudgetWings",
            FlightNumber = "BW101",
            OriginAirportCode = "EZE",
            DestinationAirportCode = "COR",
            DepartureTime = DateTime.UtcNow,
            ArrivalTime = DateTime.UtcNow,
            CabinClass = CabinClass.Economy,
            Passengers = [],
            PricePerPassenger = 100m,
            TotalPrice = 100m,
        };
        _bookingServiceMock.Setup(s => s.ConfirmBooking(request)).Returns(booking);

        var result = _controller.CreateBooking(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
        var response = Assert.IsType<CreateBookingResponse>(createdResult.Value);
        Assert.Equal("SKY-ABC123", response.BookingReferenceCode);
    }

    [Fact]
    public void CreateBooking_FlightNotFound_Returns400WithError()
    {
        var request = new CreateBookingRequest("BudgetWings", "BW101", []);
        _bookingServiceMock.Setup(s => s.ConfirmBooking(request))
            .Throws(new ArgumentException("Flight BW101 from BudgetWings is no longer available."));

        var result = _controller.CreateBooking(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("no longer available", badRequest.Value!.ToString());
    }

    [Fact]
    public void CreateBooking_InternationalRouteWithNationalId_Returns400WithError()
    {
        var request = new CreateBookingRequest("BudgetWings", "BW101", new List<CreatePassengerRequest>
        {
            new("Jane Doe", "jane@test.com", DocumentType.NationalId, "12345678"),
        });
        _bookingServiceMock.Setup(s => s.ConfirmBooking(request))
            .Throws(new InvalidOperationException("must provide a Passport Number"));

        var result = _controller.CreateBooking(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Passport", badRequest.Value!.ToString());
    }

    [Fact]
    public void CreateBooking_UnexpectedException_Returns500()
    {
        var request = new CreateBookingRequest("BudgetWings", "BW101", new List<CreatePassengerRequest>
        {
            new("John Doe", "john@test.com", DocumentType.NationalId, "12345678"),
        });
        _bookingServiceMock.Setup(s => s.ConfirmBooking(request))
            .Throws(new Exception("unexpected"));

        var result = _controller.CreateBooking(request);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }
}
