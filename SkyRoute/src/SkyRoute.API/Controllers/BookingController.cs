using Microsoft.AspNetCore.Mvc;
using SkyRoute.Application.Features.Booking;
using SkyRoute.Application.Interfaces;

namespace SkyRoute.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateBookingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CreateBooking([FromBody] CreateBookingRequest request)
    {
        try
        {
            var result = _bookingService.ConfirmBooking(request);
            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            var response = new CreateBookingResponse
            {
                BookingReferenceCode = result.Value!.ReferenceCode
            };

            return CreatedAtAction(nameof(CreateBooking), response);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "An unexpected error occurred while processing your request." });
        }
    }
}
