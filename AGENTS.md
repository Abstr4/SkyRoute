# SkyRoute — Agent Instructions

Flight search & booking aggregator. Monorepo with two projects:
- `SkyRoute/` — .NET 10 ASP.NET Core Web API
- `SkyRoute.UI/` — Angular 21 SPA (standalone components, Angular Material)

## Commands

| Area | Command | What it does |
|------|---------|-------------|
| Backend | `dotnet build` | Build solution |
| Backend | `dotnet test` | Run xUnit + Moq tests |
| Backend | `dotnet run` | API at http://localhost:5133; Scalar UI at /scalar/v1 |
| Frontend | `bun start` | Dev server at http://localhost:4200 |
| Frontend | `bun test` | Vitest unit tests |
| Frontend | `bun build` | Prod build |
| Frontend | `bunx prettier --write .` | Format (prettier: printWidth 100, singleQuote) |

## Architecture

Backend: 4-layer Clean Architecture — Domain → Application → Infrastructure → API.
- Provider pattern: `IFlightProvider` (BudgetWings, GlobalAir) registered as scoped services
- All DI in `Program.cs` (no `DependencyInjection.cs` files exist yet)
- In-memory data: `MockDataStore` (8 airports AR/BR/CL/PE, 6 BudgetWings + 10 GlobalAir flights)
- Flight IDs overlap across providers (both use 1..N)
- Pricing: BudgetWings = `max(baseFare × 0.9, 29.99)`; GlobalAir = `baseFare × 1.15`
- Booking ref: `SKY-{first 6 chars of GUID}`
- International check: compares `Country` (string), not `CountryCode`

Frontend: signals + `computed()`, `OnPush`, reactive forms, `inject()` not constructor injection.

## API endpoints

| Method | Route | Notes |
|--------|-------|-------|
| GET | `/api/Flights` | Search flights (query: `FlightSearchRequest`) |
| POST | `/api/Booking` | Not `/booking` — the `.http` file is wrong |
| GET | `/scalar/v1` | Scalar API UI (not Swagger), dev only |

## Gotchas

- CORS allows only `http://localhost:4200`
- Frontend hardcodes `https://localhost:7229` (HTTPS) — no env config
- Flight search uses `OriginAirportCode`/`DestinationAirportCode`; booking body expects `Provider`, `FlightNumber`
- Frontend search form navigates to `/flights` with query parameters; Flights component reads them and calls the API
- `.editorconfig`: 2-space indent, single quotes for TS
- Specs reference .NET 9/8 but actual is .NET 10; specs say GET but impl is POST
- Existing `SkyRoute/AGENTS.md` and `SkyRoute.UI/AGENTS.md` have per-project detail
