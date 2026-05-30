# SkyRoute — Agent Instructions

.NET 10 ASP.NET Core Web API for flight search and booking.

## Commands

- `dotnet build` — build the solution
- `dotnet test` — run all unit tests
- `dotnet run` — run API on http://localhost:5133
- Scalar API UI: http://localhost:5133/scalar/v1 (Development only)

## Endpoints

- `GET /api/Flights` — search flights (query: FlightSearchRequest)
- `POST /api/Booking` — confirm a booking (body: CreateBookingRequest)

## Architecture

- **4-layer Clean Architecture**: `SkyRoute.Domain` → `SkyRoute.Application` → `SkyRoute.Infrastructure` → `SkyRoute.API`
- **Interface/implementation separation**: services have interfaces (`IFlightSearchService`, `IBookingService`, `IFlightProvider`)
- **Provider pattern**: `IFlightProvider` implemented by `BudgetWingsProvider` + `GlobalAirProvider`, both registered as scoped services
- `FlightSearchService` aggregates `IEnumerable<IFlightProvider>` — adding a new provider requires registering it in `Program.cs`
- All DI registration is in `Program.cs` (no `DependencyInjection.cs` files yet)
- `MockDataStore` has 8 hardcoded airports (Argentina/Brazil/Chile/Peru)
- Providers have overlapping hardcoded flight IDs — IDs are not unique across providers

## Layer dependencies

- `SkyRoute.Domain` — no project dependencies (models, enums)
- `SkyRoute.Application` — depends on `SkyRoute.Domain` (interfaces, services, contracts, DTOs)
- `SkyRoute.Infrastructure` — depends on `SkyRoute.Application` (implements interfaces)
- `SkyRoute.API` — depends on `SkyRoute.Application` + `SkyRoute.Infrastructure` (composition root)

## Testing

- **Framework**: xUnit + Moq, in `src/SkyRoute.Test/SkyRoute.Test.csproj`
- **Naming**: `MethodName_Scenario_ExpectedBehavior`
- **Structure**: Arrange-Act-Assert (AAA), constructor for setup
- **Mocking**: Moq for `IFlightProvider` and service dependencies
- **Data-driven**: `[Theory]` / `[InlineData]` for parameterized tests
- Test classes match the class under test (e.g., `FlightSearchServiceTests`)
- Run with `dotnet test`

## Notes

- Uses `Scalar.AspNetCore` (not Swagger) for OpenAPI
- `SkyRoute.http` has sample search + booking requests
