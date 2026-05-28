# SkyRoute — Agent Instructions

.NET 10 ASP.NET Core Web API for flight search and booking.

## Commands

- `dotnet build` — build the solution
- `dotnet run` — run API on http://localhost:5133
- Scalar API UI: http://localhost:5133/scalar/v1 (Development only)

## Endpoints

- `POST /api/flights/search` — search flights (body: FlightSearchRequest)
- `POST /booking` — confirm a booking (body: CreateBookingRequest)

## Architecture

- **Provider pattern**: `IFlightProvider` implemented by `BudgetWingsProvider` + `GlobalAirProvider`, both registered as scoped services
- `FlightSearchService` aggregates `IEnumerable<IFlightProvider>` — adding a new provider requires registering it in `Program.cs`
- `FlightOfferRepository` is singleton; stores offers in-memory between search and booking (no DB)
- `MockDataStore` has 6 hardcoded airports (Argentina/Brazil/Chile)
- Providers have overlapping hardcoded flight IDs — IDs are not unique across providers

## Notes

- No test project exists
- Uses `Scalar.AspNetCore` (not Swagger) for OpenAPI
- `SkyRoute.http` references `/weatherforecast/` — stale, remove before using
- Controllers use inconsistent routing: `[Route("api/[controller]")]` (Flights) vs `[Route("[controller]")]` (Booking)
