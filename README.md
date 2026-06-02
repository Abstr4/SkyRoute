<picture>
  <source media="(prefers-color-scheme: dark)" srcset="SkyRoute.UI/public/favicon.ico">
  <img alt="SkyRoute" src="SkyRoute.UI/public/favicon.ico" width="64" align="left">
</picture>

# SkyRoute

> Flight search & booking aggregator

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&style=flat-square)](https://dotnet.microsoft.com)
[![Angular](https://img.shields.io/badge/Angular-21-E13137?logo=angular&style=flat-square)](https://angular.dev)

---

## Overview

SkyRoute aggregates flight offers from multiple airline providers and lets users search, compare, and book in a single interface. A monorepo with two projects:

- **SkyRoute** — a .NET 10 ASP.NET Core Web API (Clean Architecture)
- **SkyRoute.UI** — an Angular 21 SPA (standalone components, Angular Material, signals)

Flights are served from an in-memory data store with two mock providers (BudgetWings and GlobalAir), each applying their own pricing rules. The frontend provides a three-step flow: search → results → booking, with dynamic document validation based on whether the route is domestic or international.

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Bun](https://bun.sh)
- Node.js 20+

### Run the API

```bash
cd SkyRoute/src/SkyRoute.API
dotnet run --launch-profile https
```

The API starts on `https://localhost:7229`. Open `/scalar/v1` in a browser for the interactive API reference (development only).

### Run the UI

```bash
cd SkyRoute.UI
bun install
bun start
```

Opens at `http://localhost:4200`. The CORS policy only allows this origin.

> [!NOTE]
> The frontend hardcodes the API URL to `https://localhost:7229`. If you change the backend port, update `SkyRoute.UI/src/environments/environment*.ts`.

### Run Tests

```bash
# Backend (xUnit + Moq)
dotnet test SkyRoute/src/SkyRoute.Test

# Frontend (Vitest)
cd SkyRoute.UI && bun test
```

### Test scenarios

The [SkyRoute.http](SkyRoute/src/SkyRoute.API/SkyRoute.http) file contains a comprehensive set of test cases covering flight search (valid routes, empty results, validation errors) and booking (domestic/international document rules, provider errors).

---

## Architecture

### Backend — 4-layer Clean Architecture

```
SkyRoute.Domain             — models, enums (zero dependencies)
       │
SkyRoute.Application        — interfaces (ports), services (use cases), DTOs
    ╱              ╲
SkyRoute.Infrastructure     SkyRoute.API
(providers, data store)     (controllers, DI composition root)
```

| Layer | Responsibility |
|-------|---------------|
| **Domain** | Core business models (`Airport`, `Flight`, `Booking`, `Passenger`) and enums (`CabinClass`, `DocumentType`). No project dependencies. |
| **Application** | Service interfaces, use-case implementations, the `IFlightProvider` contract, request/response DTOs. Depends only on Domain. |
| **Infrastructure** | Concrete provider implementations (`BudgetWingsProvider`, `GlobalAirProvider`), the `MockDataStore`, mappers. Depends only on Application. |
| **API** | ASP.NET controllers, DI registration, CORS, middleware. The composition root. |

#### Provider pattern

`IFlightProvider` defines `Search()` and `GetByFlightNumber()`. Each provider registers as a scoped service; `FlightSearchService` receives `IEnumerable<IFlightProvider>` via DI and iterates over all registered providers. Adding a new airline requires only implementing the interface and registering it in `Program.cs`.

**Pricing rules:**

- **BudgetWings:** `max(baseFare × 0.9, 29.99)` — 10% discount, $29.99 minimum
- **GlobalAir:** `baseFare × 1.15` — 15% fuel surcharge, 2-decimal rounding

#### Mock data

| Data | Count |
|------|-------|
| Airports | 8 (4 countries) |
| BudgetWings flights | 14 |
| GlobalAir flights | 15 |

Flights span today, tomorrow, and 3 days ahead across multiple routes. Providers filter departed flights (`DepartureTime > DateTimeOffset.UtcNow`) so expired flights are never returned.

### Frontend — Component tree and data flow

```
App (router-outlet)
 ├── FlightSearchComponent   /       — search form, navigates to /flights with query params
 ├── Flights                 /flights — results table with MatSort, client-side sorting
 └── Booking                 /booking — dynamic passenger forms with document-type validation
```

1. **FlightSearchComponent** collects form values and navigates to `/flights?originAirportCode=...&destinationAirportCode=...&departureDate=...&passengers=...&cabinClass=...&timeZone=...`
2. **Flights** reads query params, calls `GET /api/Flights`, displays a `MatTable` with client-side sorting; on selection, navigates to `/booking` with the flight in router state
3. **Booking** renders the flight summary, price breakdown, and dynamic passenger forms; after successful submission, displays the booking reference code

> [!TIP]
> Search query parameters survive page refresh — the Flights component re-fetches from the API. Navigating to `/booking` directly (or refreshing) loses the selected flight.

---

## Design Decisions

| Decision | Rationale |
|----------|-----------|
| **DI extension methods per layer** | Each layer exposes an `IServiceCollection` extension method (`AddPresentation()`, `AddInfrastructure()`, `AddApplication()`) so `Program.cs` only references layer entry points, not individual types. Registration logic stays co-located with the types it registers; adding a new service in a layer only touches that layer's `DependencyInjection.cs`. |
| **GET with query params for flight search** | Idempotent, cacheable, bookmarkable. The five scalar parameters fit comfortably in a URL. |
| **Provider pattern (Strategy)** | New providers added by implementing `IFlightProvider` and registering in DI — zero changes to existing controllers or services. |
| **4-layer Clean Architecture** | Separates domain logic, use cases, infrastructure, and presentation. Enables unit testing with mocked dependencies at each layer. |
| **Query params for search, router state for booking** | Search results survive page refresh; the selected flight is transient and naturally expires. |
| **Timezone-aware day boundaries** | The backend converts the user's IANA timezone to UTC boundaries before querying flights, preventing false "past date" rejections for users in negative-offset timezones. |
| **Result pattern + global exception handler** | `Result<T>` return types keep expected failures (validation, not-found) explicit in the domain layer rather than throwing exceptions for control flow. The `GlobalExceptionHandler` catches remaining unexpected exceptions and returns structured `application/problem+json` responses. |
| **`IExceptionHandler` over custom middleware** | Uses the built-in `IExceptionHandler` API registered via `AddExceptionHandler<T>()` instead of a custom middleware pipeline — produces `ProblemDetails` out of the box, aligning with RFC 9457 problem response conventions. |
| **Re-seed on startup over EF Core `HasData()`** | `HasData()` bakes static seed data into migrations at migration time. Flight departure times must be relative to "now" (today, tomorrow, +3 days) so expired-flight filtering and test scenarios remain valid across restarts. Re-seeding on application start keeps dates current. |
| **Scalar over Swagger** | A more modern, readable API reference UI with minimal configuration via `Scalar.AspNetCore`. |

---

## Known Limitations

- **Flight ID collision** — Both providers use overlapping numeric IDs (1..N). IDs are unique only within a provider, not globally.
- **No pagination** — Flight search returns all matching results. Would need pagination for larger datasets.
- **`timeZone` is required** — The search endpoint requires an IANA timezone. The frontend sends `Intl.DateTimeFormat().resolvedOptions().timeZone` automatically; manual API callers must include one.
- **Booking state lost on refresh** — Router state on `/booking` doesn't survive a page refresh. Navigating to `/booking` directly redirects home.
- **Hardcoded API URL** — The frontend uses `https://localhost:7229` in all environments. No per-environment switching.
- **Backend state is ephemeral** — Bookings are stored in a private `List<Booking>` inside `BookingService` and are lost on server restart.
- **CORS restricted to `localhost:4200`** — Hardcoded for the Angular dev server. Any other client must be added explicitly.
- **Logging in controllers, not filters** — `ILogger<T>` is injected directly into controllers. Extracting cross-cutting concerns into action filters (or a CQRS pipeline with behaviors) would be cleaner.
- **No async I/O** — All data is served from a static in-memory `MockDataStore`; there are no `async` code paths. A separate branch is in progress to replace it with EF Core InMemory and true async operations.

---

## Project Structure

```
├── SkyRoute/                     # .NET 10 backend
│   └── src/
│       ├── SkyRoute.Domain/      # Models, enums (zero deps)
│       ├── SkyRoute.Application/ # Interfaces, services, DTOs
│       ├── SkyRoute.Infrastructure/ # Providers, data store, mappers
│       ├── SkyRoute.API/         # Controllers, Program.cs, DI
│       └── SkyRoute.Test/        # xUnit + Moq unit tests
├── SkyRoute.UI/                  # Angular 21 frontend
│   └── src/app/
│       ├── flight-search/        # Search form page
│       ├── flights/              # Results table page
│       ├── booking/              # Booking form page
│       ├── services/             # API service classes
│       └── models/               # Shared interfaces, constants
├── specs/                        # Original challenge specification
└── AGENTS.md
```
