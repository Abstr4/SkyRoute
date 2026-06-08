using Microsoft.AspNetCore.Mvc;
using SkyRoute.Application.Features.Booking;
using SkyRoute.Application.Features.Flights;
using SkyRoute.Application.Interfaces;

namespace SkyRoute.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class FlightsController : ControllerBase
    {
        private readonly IFlightSearchService _flightSearchService;
        private readonly ILogger<FlightsController> _logger;

        public FlightsController(
            IFlightSearchService flightSearchService,
            ILogger<FlightsController> logger)
        {
            _flightSearchService = flightSearchService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FlightSearchResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchFlights([FromQuery] FlightSearchRequest request)
        {
            _logger.LogInformation(
                "Flight search requested: {Origin} -> {Destination} on {Date}, {Passengers} passenger(s)",
                request.OriginAirportCode, request.DestinationAirportCode,
                request.DepartureDate, request.Passengers);

            var result = await _flightSearchService.Search(request);
            if (result.IsFailure)
            {
                _logger.LogWarning(
                    "Flight search validation failed: {Errors}", result.Errors);
                return BadRequest(new { errors = result.Errors });
            }

            _logger.LogInformation(
                "Flight search returned {Count} result(s)", result.Value!.Count);

            return Ok(result.Value);
        }
    }
}
