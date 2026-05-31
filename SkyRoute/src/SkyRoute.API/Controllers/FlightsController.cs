using Microsoft.AspNetCore.Mvc;
using SkyRoute.Application.Contracts.Requests;
using SkyRoute.Application.Interfaces;

namespace SkyRoute.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class FlightsController : ControllerBase
    {
        private readonly IFlightSearchService _flightSearchService;

        public FlightsController(IFlightSearchService flightSearchService)
        {
            _flightSearchService = flightSearchService;
        }

        [HttpGet]
        public IActionResult SearchFlights([FromQuery] FlightSearchRequest request)
        {
            TimeZoneInfo tz;
            try
            {
                tz = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);
            }
            catch
            {
                return BadRequest("Invalid timezone.");
            }

            var localStart = request.DepartureDate.ToDateTime(TimeOnly.MinValue);
            var localEnd = localStart.AddDays(1);
            var utcStart = TimeZoneInfo.ConvertTimeToUtc(localStart, tz);
            var utcEnd = TimeZoneInfo.ConvertTimeToUtc(localEnd, tz);

            if (utcEnd <= DateTimeOffset.UtcNow)
            {
                return BadRequest("Departure date cannot be in the past.");
            }

            if (string.Equals(request.OriginAirportCode, request.DestinationAirportCode, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Origin and destination airports cannot be the same.");
            }

            try
            {
                var searchResults = _flightSearchService.Search(request, utcStart, utcEnd);
                return Ok(searchResults);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your search: {ex.Message}");
            }
        }
    }
}