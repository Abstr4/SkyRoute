using SkyRoute.Application.Features.Flights;

namespace SkyRoute.Application.Interfaces;

public interface IFlightSearchService
{
    IReadOnlyList<FlightSearchResponse> Search(FlightSearchRequest request, DateTimeOffset utcStart, DateTimeOffset utcEnd);
}
