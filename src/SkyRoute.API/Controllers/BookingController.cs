using Microsoft.AspNetCore.Mvc;
using SkyRoute.API.Contracts.Requests;
using SkyRoute.API.Contracts.Responses;
using SkyRoute.API.Services;

namespace SkyRoute.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BookingController : ControllerBase
{
    private readonly BookingService _bookingService;

    public BookingController(BookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost(Name = "CreateBooking")]
    [ProducesResponseType(typeof(CreateBookingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CreateBooking([FromBody] CreateBookingRequest request)
    {
        try
        {
            var booking = _bookingService.ConfirmBooking(request);

            var response = new CreateBookingResponse
            {
                BookingReferenceCode = booking.ReferenceCode
            };

            return CreatedAtAction(nameof(CreateBooking), response);
        }
        catch (ArgumentException ex)
        {
            // Flight not found or invalid airport data
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // Document type validation failed
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = "An unexpected error occurred while processing your booking request." });
        }
    }
}
