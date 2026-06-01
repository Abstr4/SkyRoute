using Microsoft.AspNetCore.Mvc;
using SkyRoute.Application.Features.Flights;
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
            try
            {
                var result = _flightSearchService.Search(request);
                if (!result.IsSuccess)
                    return BadRequest(new { errors = result.Errors });

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your search: {ex.Message}");
            }
        }
    }
}
