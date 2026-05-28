using Microsoft.AspNetCore.Mvc;
using SkyRoute.API.Contracts.Requests;
using SkyRoute.API.Services;

namespace SkyRoute.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class FlightsController : ControllerBase
    {
        private readonly FlightSearchService _flightSearchService;

        public FlightsController(FlightSearchService flightSearchService)
        {
            _flightSearchService = flightSearchService;
        }

        [HttpPost("search")]
        public IActionResult SearchFlights([FromBody] FlightSearchRequest request)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (request.DepartureDate < today)
            {
                return BadRequest("Departure date cannot be in the past.");
            }

            if (string.Equals(request.OriginAirportCode, request.DestinationAirportCode, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Origin and destination airports cannot be the same.");
            }

            try
            {
                var searchResults = _flightSearchService.Search(request);
                return Ok(searchResults);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your search: {ex.Message}");
            }
        }
    }
}