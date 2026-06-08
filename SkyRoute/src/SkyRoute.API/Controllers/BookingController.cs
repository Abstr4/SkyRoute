using Microsoft.AspNetCore.Mvc;
using SkyRoute.Application.Features.Booking;
using SkyRoute.Application.Interfaces;

namespace SkyRoute.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingController> _logger;

    public BookingController(
        IBookingService bookingService,
        ILogger<BookingController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateBookingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CreateBooking([FromBody] CreateBookingRequest request)
    {
        _logger.LogInformation(
            "Booking requested: {Provider} flight {FlightNumber}, {Passengers} passenger(s)",
            request.Provider, request.FlightNumber, request.Passengers?.Count ?? 0);

        var result = _bookingService.ConfirmBooking(request);
        if (result.IsFailure)
        {
            _logger.LogWarning(
                "Booking failed: {Errors}", result.Errors);
            return BadRequest(new { errors = result.Errors });
        }

        var response = new CreateBookingResponse
        {
            BookingReferenceCode = result.Value!.ReferenceCode
        };

        _logger.LogInformation(
            "Booking confirmed: {ReferenceCode}", response.BookingReferenceCode);

        return Created(string.Empty, response);
    }
}
