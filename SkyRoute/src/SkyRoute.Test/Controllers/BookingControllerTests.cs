using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SkyRoute.Application.Common;
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
        _controller = new BookingController(_bookingServiceMock.Object, Mock.Of<ILogger<BookingController>>());
    }

    [Fact]
    public async Task CreateBooking_ValidDomesticRequest_Returns201WithReferenceCode()
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
        _bookingServiceMock.Setup(s => s.ConfirmBooking(request))
            .ReturnsAsync(Result<Booking>.Success(booking));

        var result = await _controller.CreateBooking(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
        var response = Assert.IsType<CreateBookingResponse>(createdResult.Value);
        Assert.Equal("SKY-ABC123", response.BookingReferenceCode);
    }

    [Fact]
    public async Task CreateBooking_FlightNotFound_Returns400WithError()
    {
        var request = new CreateBookingRequest("BudgetWings", "BW101", []);
        _bookingServiceMock.Setup(s => s.ConfirmBooking(request))
            .ReturnsAsync(Result<Booking>.Failure("Flight BW101 from BudgetWings is no longer available."));

        var result = await _controller.CreateBooking(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task CreateBooking_InternationalRouteWithNationalId_Returns400WithError()
    {
        var request = new CreateBookingRequest("BudgetWings", "BW101", new List<CreatePassengerRequest>
        {
            new("Jane Doe", "jane@test.com", DocumentType.NationalId, "12345678"),
        });
        _bookingServiceMock.Setup(s => s.ConfirmBooking(request))
            .ReturnsAsync(Result<Booking>.Failure("must provide a Passport Number"));

        var result = await _controller.CreateBooking(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task CreateBooking_ValidationFailure_ReturnsBadRequest()
    {
        var request = new CreateBookingRequest("BudgetWings", "BW101", new List<CreatePassengerRequest>
        {
            new("John Doe", "john@test.com", DocumentType.NationalId, "12345678"),
        });
        _bookingServiceMock.Setup(s => s.ConfirmBooking(request))
            .ReturnsAsync(Result<Booking>.Failure("Invalid request."));

        var result = await _controller.CreateBooking(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }


}
