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