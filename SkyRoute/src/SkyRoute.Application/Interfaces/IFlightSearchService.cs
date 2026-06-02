using SkyRoute.Application.Common;
using SkyRoute.Application.Features.Flights;

namespace SkyRoute.Application.Interfaces;

public interface IFlightSearchService
{
    Task<Result<IReadOnlyList<FlightSearchResponse>>> Search(FlightSearchRequest request);
}
