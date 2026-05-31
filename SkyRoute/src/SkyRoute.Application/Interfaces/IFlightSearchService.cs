using SkyRoute.Application.Contracts.Requests;
using SkyRoute.Application.Contracts.Responses;

namespace SkyRoute.Application.Interfaces;

public interface IFlightSearchService
{
    IReadOnlyList<FlightSearchResponse> Search(FlightSearchRequest request, DateTimeOffset utcStart, DateTimeOffset utcEnd);
}
