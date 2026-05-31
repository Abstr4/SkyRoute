<picture>
  <source media="(prefers-color-scheme: dark)" srcset="SkyRoute.UI/public/favicon.ico">
  <img alt="SkyRoute" src="SkyRoute.UI/public/favicon.ico" width="64" align="left">
</picture>

# SkyRoute

> Flight search & booking aggregator

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&style=flat-square)](https://dotnet.microsoft.com)
[![Angular](https://img.shields.io/badge/Angular-21-E13137?logo=angular&style=flat-square)](https://angular.dev)

[Overview](#overview) • [Features](#features) • [Getting Started](#getting-started) • [Architecture](#architecture) • [API](#api) • [Project Structure](#project-structure) • [Design Decisions](#design-decisions) • [Known Limitations](#known-limitations)

---

## Overview

SkyRoute aggregates flight offers from multiple airline providers and lets users search, compare, and book in a single interface. Built as a demonstration monorepo with two projects:

- **SkyRoute** — a .NET 10 ASP.NET Core Web API following Clean Architecture
- **SkyRoute.UI** — an Angular 21 SPA using standalone components, Angular Material, signals, and reactive forms

Flights are served from an in-memory data store with two mock providers (BudgetWings and GlobalAir), each applying their own pricing rules. The frontend provides a three-step flow: search → results → booking, with dynamic document validation based on whether the route is domestic or international.

---

## Features

- **Multi-provider aggregation** — searches across all registered providers and merges results into a single response
- **Provider-agnostic pricing engine** — each provider implements its own pricing strategy via a shared interface
- **Interactive flight search** — origin, destination, date, passenger count, and cabin class selection
- **Client-side sorting** — sort results by price (asc/desc), duration, or departure time without additional API calls
- **Dynamic document validation** — detects international vs domestic routes and adjusts the required document type (passport or national ID)
- **Reactive booking form** — dynamic passenger forms with per-person validation
- **OpenAPI UI** — interactive API reference at `/scalar/v1` using Scalar (development only)

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Bun](https://bun.sh) (package manager & runtime)
- Node.js 20+ _(required by Angular tooling)_

### Run the API

```bash
cd SkyRoute
dotnet run
```

The API starts on `http://localhost:5133`. Open `http://localhost:5133/scalar/v1` in a browser to explore the API interactively.

### Run the UI

```bash
cd SkyRoute.UI
bun install
bun start
```

The UI starts on `http://localhost:4200`. The CORS policy only allows this origin.

> [!NOTE]
> The frontend hardcodes the API URL to `https://localhost:7229` (the HTTPS port from the backend's `launchSettings.json`). If you change the backend port, update the URL in the frontend services.

### Run tests

```bash
# Backend tests (xUnit + Moq)
cd SkyRoute
dotnet test

# Frontend tests (Vitest)
cd SkyRoute.UI
bun test
```

---

## Architecture

### Backend — 4-layer Clean Architecture

```
          SkyRoute.Domain          — models, enums (zero dependencies)
                 │
      SkyRoute.Application     — interfaces (ports), services (use cases), DTOs, contracts
          ╱               ╲
         ╱                 ╲
        ╱                   ╲
SkyRoute.Infrastructure     SkyRoute.API
(providers, data store)     (controllers, DI composition root)
```

| Layer | Responsibility |
|-------|---------------|
| **Domain** | Core business models (`Airport`, `Flight`, `Booking`, `Passenger`) and enums (`CabinClass`, `DocumentType`). No project dependencies. |
| **Application** | Service interfaces (`IFlightSearchService`, `IBookingService`), use-case implementations (`FlightSearchService`, `BookingService`), the `IFlightProvider` contract, request/response DTOs. Depends only on Domain. |
| **Infrastructure** | Concrete provider implementations (`BudgetWingsProvider`, `GlobalAirProvider`), the `MockDataStore` with hardcoded airports and flights. Depends only on Application. |
| **API** | ASP.NET controllers (`FlightsController`, `BookingController`), DI registration, CORS, and middleware. Composition root that wires everything together. |

#### Provider pattern

The `IFlightProvider` interface defines `Search()` and `GetByFlightNumber()` methods. Each provider registers as a scoped `IFlightProvider` service in `Program.cs`. `FlightSearchService` receives `IEnumerable<IFlightProvider>` via DI and iterates over all registered providers for every search query — adding a new provider requires only implementing the interface and registering it.

**Pricing rules:**

- **BudgetWings:** `max(baseFare × 0.9, 29.99)` — 10% discount, minimum $29.99 per passenger
- **GlobalAir:** `baseFare × 1.15` — 15% fuel surcharge, rounded to 2 decimal places

#### Mock data

| Data | Count |
|------|-------|
| Airports | 8 (Argentina, Brazil, Chile, Peru) |
| BudgetWings flights | 14 |
| GlobalAir flights | 15 |

Flights span **today**, **tomorrow**, and **3 days from now** across multiple routes. Each provider applies **`f.DepartureTime > DateTimeOffset.UtcNow`** so expired flights are never returned — simulating real-world provider behavior.

Flight IDs are not unique across providers (both start at 1). The `BookingService` validates document types against route internationality by comparing the `Country` string field.

### Frontend — Component tree and data flow

```
App (router-outlet)
 ├── FlightSearchComponent    /   — search form, submits GET /api/Flights
 ├── Flights                  /flights   — results table with MatSort
 └── Booking                  /booking   — dynamic passenger forms, submits POST /api/Booking
```

Data flows between components exclusively through **Router state** (`router.getCurrentNavigation().extras.state`):

1. `FlightSearchComponent` collects form values, calls the API, and navigates to `/flights` with `{ results, passengers }` in state
2. `Flights` displays results in a `MatTable` with client-side sorting; on selection navigates to `/booking` with `{ flight, passengers }`
3. `Booking` renders the flight summary, price breakdown, and dynamic passenger forms; after successful submission displays the booking reference code

> [!TIP]
> Because data lives in router state, refreshing the browser on `/flights` or `/booking` will lose the state and redirect to the search page. This is a known trade-off (see Limitations).

---

## API

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/Flights` | Search available flights |
| `POST` | `/api/Booking` | Confirm a booking |
| `GET` | `/scalar/v1` | Scalar API reference (dev only) |

### `GET /api/Flights`

**Query parameters:**

| Parameter | Type | Required | Example |
|-----------|------|----------|---------|
| `originAirportCode` | `string` | yes | `EZE` |
| `destinationAirportCode` | `string` | yes | `GRU` |
| `departureDate` | `string` (ISO date) | yes | `2026-06-15` |
| `passengers` | `int` | yes | `2` |
| `cabinClass` | `string` | yes | `Economy` |
| `timeZone` | `string` (IANA) | yes | `America/Argentina/Cordoba` |

The `timeZone` parameter is required. The backend converts the user's local day into UTC boundaries and filters flights against them. Use [IANA timezone names](https://en.wikipedia.org/wiki/List_of_tz_database_time_zones) (e.g., `UTC`, `America/Argentina/Cordoba`, `America/Sao_Paulo`).

**Example request:**

```
GET /api/Flights?originAirportCode=EZE&destinationAirportCode=GRU&departureDate=2026-06-15&passengers=2&cabinClass=Economy&timeZone=UTC
```

**Response:** `200 OK`

```json
[
  {
    "provider": "BudgetWings",
    "flightNumber": "BW404",
    "originAirport": { "code": "EZE", "name": "...", "city": "Buenos Aires", "country": "Argentina", "countryCode": "AR" },
    "destinationAirport": { "code": "GRU", "name": "...", "city": "São Paulo", "country": "Brazil", "countryCode": "BR" },
    "departureTime": "2026-06-15T07:00:00Z",
    "arrivalTime": "2026-06-15T10:15:00Z",
    "durationMinutes": 195,
    "cabinClass": "Economy",
    "pricePerPassenger": 108.00,
    "totalPrice": 216.00
  }
]
```

### Test Scenarios

All examples use `timeZone=UTC` unless noted. Replace `departureDate` with the appropriate relative date.

#### Search — flights expected

| # | Scenario | Query | Expected |
|---|----------|-------|----------|
| 1 | **COR→MDZ today** (10 flights, 5 per provider) | `origin=COR&dest=MDZ&date=today&tz=America/Argentina/Cordoba` | 7-10 results (some morning flights may expire depending on time of day) |
| 2 | **EZE→GRU tomorrow** (4 flights, 2 per provider) | `origin=EZE&dest=GRU&date=tomorrow&tz=UTC` | 4 results (BW404, BW408, GA401, GA402) |
| 3 | **GRU→EZE tomorrow** (4 flights, 2 per provider) | `origin=GRU&dest=EZE&date=tomorrow&tz=UTC` | 4 results (BW707, BW708, GA305, GA306) |
| 4 | **EZE→SCL in 3 days** (3 flights, 1 BW + 2 GA) | `origin=EZE&dest=SCL&date=in+3+days&tz=UTC` | 3 results (BW505, GA510, GA511) |
| 5 | **EZE→COR today** (1 flight) | `origin=EZE&dest=COR&date=today&tz=UTC` | 1 result (BW101) |
| 6 | **GRU→GIG today** (1 flight) | `origin=GRU&dest=GIG&date=today&tz=UTC` | 1 result (GA102) |
| 7 | **EZE→MDZ today** (1 flight) | `origin=EZE&dest=MDZ&date=today&tz=UTC` | 1 result (BW202, may be expired after 12:10 UTC) |

#### Search — empty / error responses

| # | Scenario | Query | Expected |
|---|----------|-------|----------|
| 8 | **Past date** (yesterday) | `origin=COR&dest=MDZ&date=yesterday&tz=UTC` | `400 Bad Request` — date cannot be in the past |
| 9 | **Same origin and destination** | `origin=EZE&dest=EZE&date=tomorrow&tz=UTC` | `400 Bad Request` — airports cannot be the same |
| 10 | **Invalid timezone** | `origin=EZE&dest=GRU&date=tomorrow&tz=Foo/Bar` | `400 Bad Request` — invalid timezone |
| 11 | **Unknown route** (no flights) | `origin=AEP&dest=LIM&date=today&tz=UTC` | `200 OK` — empty array `[]` |
| 12 | **Far future** (no flights scheduled) | `origin=EZE&dest=GRU&date=in+30+days&tz=UTC` | `200 OK` — empty array `[]` |
| 13 | **Date with no matching flights** | `origin=GIG&dest=LIM&date=today&tz=UTC` | `200 OK` — empty array `[]` |
| 14 | **All flights expired today** | `origin=EZE&dest=COR&date=today&tz=UTC` (test after 08:10 UTC) | `200 OK` — empty array (BW101 already departed) |

#### Booking — success scenarios

| # | Scenario | Body | Expected |
|---|----------|------|----------|
| 15 | **Domestic route** (COR→MDZ) | `{ provider: "BudgetWings", flightNumber: "BW310", passengers: [{ fullName: "Jane Doe", email: "jane@test.com", documentType: "NationalId", documentNumber: "12345678" }] }` | `201 Created` — `{ bookingReferenceCode: "SKY-..." }` |
| 16 | **International route** (EZE→GRU) | `{ provider: "BudgetWings", flightNumber: "BW404", passengers: [{ fullName: "Jane Doe", email: "jane@test.com", documentType: "Passport", documentNumber: "AB123456" }] }` | `201 Created` |

#### Booking — error scenarios

| # | Scenario | Body | Expected |
|---|----------|------|----------|
| 17 | **Domestic route with Passport** (COR→MDZ) | `{ provider: "BudgetWings", flightNumber: "BW310", passengers: [{ ..., documentType: "Passport" }] }` | `400 Bad Request` — must provide a National ID for domestic routes |
| 18 | **International route with NationalId** (EZE→GRU) | `{ provider: "BudgetWings", flightNumber: "BW404", passengers: [{ ..., documentType: "NationalId" }] }` | `400 Bad Request` — must provide a Passport for international routes |
| 19 | **Invalid provider** | `{ provider: "FakeAir", flightNumber: "BW101" }` | `400 Bad Request` |
| 20 | **Invalid flight number** | `{ provider: "BudgetWings", flightNumber: "FAKE" }` | `400 Bad Request` |

### `POST /api/Booking`

**Request body:**

```json
{
  "provider": "BudgetWings",
  "flightNumber": "BW404",
  "passengers": [
    {
      "fullName": "Jane Doe",
      "email": "jane@example.com",
      "documentType": "Passport",
      "documentNumber": "AB123456"
    }
  ]
}
```

The `documentType` must be `Passport` for international routes and `NationalId` for domestic routes.

**Response:** `201 Created`

```json
{
  "bookingReferenceCode": "SKY-A1B2C3"
}
```

---

## Project Structure

```
├── SkyRoute/                         # .NET 10 backend
│   ├── SkyRoute.slnx
│   └── src/
│       ├── SkyRoute.Domain/          # Models, enums (zero deps)
│       ├── SkyRoute.Application/     # Interfaces, services, DTOs, contracts
│       ├── SkyRoute.Infrastructure/  # Providers, MockDataStore, mappers
│       ├── SkyRoute.API/             # Controllers, Program.cs, DI
│       └── SkyRoute.Test/            # xUnit + Moq unit tests
│
├── SkyRoute.UI/                      # Angular 21 frontend
│   └── src/app/
│       ├── app.ts / app.html / app.css       # Root shell
│       ├── app.config.ts                     # App configuration
│       ├── app.routes.ts                     # Route definitions
│       ├── flight-search/                     # Search form page
│       ├── flights/                           # Results table page
│       ├── booking/                           # Booking form page
│       ├── models/                            # Shared interfaces, constants
│       └── services/                          # API service classes
│
├── specs/                             # Original challenge specification
└── AGENTS.md                          # Agent instructions (this file)
```

---

## Design Decisions

| Decision | Rationale |
|----------|-----------|
| **GET with query params for flight search** | Search queries on a resource collection should be GET — idempotent, cacheable, bookmarkable. The five scalar parameters fit comfortably in a URL. The original implementation used POST but was changed to align with REST conventions and the spec. |
| **Provider pattern (Strategy)** | `IFlightProvider` lets new airlines be added by implementing a single interface and registering in DI — zero changes to existing services or controllers. |
| **4-layer Clean Architecture** | Separates domain logic (Domain), use cases (Application), infrastructure concerns (Infrastructure), and presentation (API). Enables unit testing with mocked dependencies. |
| **Router state for data flow** | Avoids a state management library or shared service for this small app. Simple and sufficient for a linear 3-page flow — at the cost of state surviving only as long as the in-memory navigation history. |
| **`HttpClient` with `provideHttpClient(withFetch())`** | Uses Angular's standard HTTP client with the modern fetch-based adapter. Provides centralized error handling, typed responses, and integrates with RxJS via `Observable`. The `FlightSearchService` encapsulates all API calls, keeping components framework-aware but not HTTP-aware. |
| **All DI in `Program.cs`** | Keeps the composition root explicit and visible in a single file. As the project grows, `DependencyInjection.cs` extension methods per layer would be cleaner. |
| **Scalar over Swagger** | Scalar provides a more modern, readable API reference UI. Integrated via `Scalar.AspNetCore` with deep-space theme. |
| **In-memory data store** | Zero setup, no database dependency. Adequate for a demo — but all data resets on server restart, and concurrent access uses no synchronization. |
| **Timezone-aware day boundaries** | The backend converts the user's IANA timezone to UTC day boundaries before querying flights. This prevents local-midnight vs UTC-midnight mismatches that caused false "past date" rejections for users in negative-offset timezones. |
| **Provider expiry filtering** | Providers filter out flights with `DepartureTime <= DateTimeOffset.UtcNow`, matching real-world behavior where airline APIs never return departed flights. Combined with the controller's past-date validation, this ensures only bookable flights are returned. |

---

## Known Limitations

- **Flight ID collision** — Both providers use overlapping numeric IDs (1..N). IDs are unique only within a provider, not globally.
- **No pagination** — Flight search returns all matching results. With more providers or routes, this would need pagination or filtering.
- **`timeZone` is required** — The search endpoint requires an IANA timezone. The frontend sends `Intl.DateTimeFormat().resolvedOptions().timeZone` automatically. Manual API callers must include a valid timezone parameter.
- **State lost on refresh** — Router state in Angular doesn't survive a page refresh. Navigating to `/flights` directly shows an empty state; navigating to `/booking` redirects home.
- **Hardcoded API URL** — The frontend uses `https://localhost:7229` directly. There is no environment-based configuration to switch between development and production endpoints.
- **Backend state is ephemeral** — Bookings are stored in a private `List<Booking>` inside `BookingService` and are lost on restart.
- **International check uses `Country` (string), not `CountryCode`** — The `BookingService` compares `originAirport.Country` vs `destinationAirport.Country`. This works for the current data but would fail if two countries shared the same name string across different codes.
- **CORS restricted to `localhost:4200`** — The policy is hardcoded for the Angular dev server. Any other client must be added explicitly.
- **Spec vs implementation** — The original spec defines `POST /api/bookings`; the implementation uses `POST /api/Booking` (note casing difference). The search endpoint now matches the spec (`GET /api/Flights`).
- **No global loading/error state** — Only the search form shows a loading spinner. The booking and flights pages have no visual loading indicator during API calls.
- **No `ChangeDetectionStrategy.OnPush`** — The Angular components use default change detection despite the project conventions recommending `OnPush`.
- **Mixed template syntax** — The flights table uses structural directives (`*matCellDef`, `*matHeaderRowDef`) alongside the newer `@if`/`@for` control flow syntax.
