# SkyRoute Clean Architecture Migration

## Target project structure

```
src/
├── SkyRoute.Domain/                              # Layer 0 — Zero dependencies
│   ├── SkyRoute.Domain.csproj
│   └── Models/
│       ├── Airport.cs
│       ├── Booking.cs
│       ├── CabinClass.cs
│       ├── DocumentType.cs
│       ├── Flight.cs
│       └── Passenger.cs
│
├── SkyRoute.Application/                         # Layer 1 — Depends on Domain
│   ├── SkyRoute.Application.csproj
│   ├── DependencyInjection.cs
│   ├── Interfaces/
│   │   ├── IFlightProvider.cs
│   │   ├── IFlightSearchService.cs
│   │   ├── IBookingService.cs
│   │   ├── IFlightOfferRepository.cs
│   │   └── IAirportRepository.cs
│   ├── Services/
│   │   ├── FlightSearchService.cs
│   │   └── BookingService.cs
│   ├── Contracts/
│   │   ├── Requests/
│   │   │   ├── FlightSearchRequest.cs
│   │   │   ├── CreateBookingRequest.cs
│   │   │   └── CreatePassengerRequest.cs
│   │   └── Responses/
│   │       ├── FlightSearchResponse.cs
│   │       └── CreateBookingResponse.cs
│   └── DTOs/
│       ├── FlightOffer.cs
│       ├── AirportDto.cs
│       └── PassengerDetailsDto.cs
│
├── SkyRoute.Infrastructure/                       # Layer 2 — Depends on Application
│   ├── SkyRoute.Infrastructure.csproj
│   ├── DependencyInjection.cs
│   ├── Providers/
│   │   ├── BudgetWingsProvider.cs
│   │   └── GlobalAirProvider.cs
│   └── Data/
│       ├── FlightOfferRepository.cs
│       ├── AirportRepository.cs
│       └── MockDataStore.cs                       # Kept as data source, not directly referenced by Application
│
├── SkyRoute.API/                                  # Layer 3 — Presentation
│   ├── SkyRoute.API.csproj
│   ├── Program.cs
│   └── Controllers/
│       ├── FlightsController.cs
│       └── BookingController.cs
│
└── SkyRoute.Test/
    ├── SkyRoute.Test.csproj
    ├── Repositories/
    │   └── FlightOfferRepositoryTests.cs
    ├── Services/
    │   ├── FlightSearchServiceTests.cs
    │   └── BookingServiceTests.cs
    └── Controllers/
        ├── FlightsControllerTests.cs
        └── BookingControllerTests.cs
```

## Dependency flow

```
           ┌──────────────────┐
           │  SkyRoute.Domain │  ← Models, enums (zero dependencies)
           └────────┬─────────┘
                    │ references
           ┌────────▼──────────┐
           │ SkyRoute.Application│  ← Interfaces (ports), Services (use cases), DTOs, Contracts
           └────────┬──────────┘
                   ╱│╲
                  ╱ │ ╲
                 ╱  │  ╲
   ┌───────────┐   │   ┌────────────────┐
   │ Infrastructure│  │  SkyRoute.API   │  ← Presentation (controllers, Program.cs)
   └───────────┘   │   └────────────────┘
                   │
          ┌────────▼─────────┐
          │  SkyRoute.Test   │  ← References API (which transitively references all)
          └──────────────────┘
```

### Direction rules

- **Domain**: references nothing external
- **Application**: references Domain only
- **Infrastructure**: references Application only (implements its interfaces)
- **API**: references Application + Infrastructure (composition root)
- **Test**: references API (transitively gets everything)

---

## Step-by-step interface extraction

### Step 1: `IFlightOfferRepository`

**Purpose:** Decouple `FlightSearchService` and `BookingService` from the concrete repository.

**New file** — `src/SkyRoute.API/Services/IFlightOfferRepository.cs`:

```csharp
namespace SkyRoute.API.Services;

public interface IFlightOfferRepository
{
    void StoreOffer(FlightOffer offer);
    void StoreOffers(IEnumerable<FlightOffer> offers);
    FlightOffer? GetOfferById(int offerId);
    void ClearOffers();
}
```

**Changes to existing class** — `FlightOfferRepository.cs`:

```csharp
// Before: public sealed class FlightOfferRepository
// After:  public sealed class FlightOfferRepository : IFlightOfferRepository
```

Add the interface to the class signature. Body stays identical.

**Changes to `FlightSearchService.cs`:**

```csharp
// Before:
private readonly FlightOfferRepository _offerRepository;
public FlightSearchService(IEnumerable<IFlightProvider> providers, FlightOfferRepository offerRepository)

// After:
private readonly IFlightOfferRepository _offerRepository;
public FlightSearchService(IEnumerable<IFlightProvider> providers, IFlightOfferRepository offerRepository)
```

**Changes to `BookingService.cs`:**

```csharp
// Before:
private readonly FlightOfferRepository _offerRepository;
public BookingService(FlightOfferRepository offerRepository)

// After:
private readonly IFlightOfferRepository _offerRepository;
public BookingService(IFlightOfferRepository offerRepository)
```

**DI registration in `Program.cs`:**

```csharp
// Before:
builder.Services.AddSingleton<FlightOfferRepository>();

// After:
builder.Services.AddSingleton<IFlightOfferRepository, FlightOfferRepository>();
```

**Test impact:** Minimal. Tests that construct `FlightSearchService` or `BookingService` with a real `FlightOfferRepository` still work since `FlightOfferRepository : IFlightOfferRepository`. No test code changes required.

---

### Step 2: `IAirportRepository`

**Purpose:** Replace static `MockDataStore.Airports` calls with an injectable dependency, so `BookingService` no longer depends on Infrastructure directly.

**New interface** — `src/SkyRoute.API/Services/IAirportRepository.cs`:

```csharp
using SkyRoute.API.Models;

namespace SkyRoute.API.Services;

public interface IAirportRepository
{
    Airport? GetByCode(string code);
    IReadOnlyList<Airport> GetAll();
}
```

**New implementation** — `src/SkyRoute.API/Data/AirportRepository.cs`:

```csharp
using SkyRoute.API.Models;
using SkyRoute.API.Services;

namespace SkyRoute.API.Data;

public sealed class AirportRepository : IAirportRepository
{
    public Airport? GetByCode(string code)
    {
        return MockDataStore.Airports.FirstOrDefault(a =>
            string.Equals(a.Code, code, StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyList<Airport> GetAll()
    {
        return MockDataStore.Airports;
    }
}
```

**Changes to `BookingService.cs`:**

```csharp
// Before:
private readonly FlightOfferRepository _offerRepository;
public BookingService(FlightOfferRepository offerRepository)
{
    _offerRepository = offerRepository;
}

// In ConfirmBooking:
var originAirport = MockDataStore.Airports.FirstOrDefault(a => a.Code == selectedFlight.OriginAirport.Code);
var destinationAirport = MockDataStore.Airports.FirstOrDefault(a => a.Code == selectedFlight.DestinationAirport.Code);

// After:
private readonly IFlightOfferRepository _offerRepository;
private readonly IAirportRepository _airportRepository;
public BookingService(IFlightOfferRepository offerRepository, IAirportRepository airportRepository)
{
    _offerRepository = offerRepository;
    _airportRepository = airportRepository;
}

// In ConfirmBooking:
var originAirport = _airportRepository.GetByCode(selectedFlight.OriginAirport.Code);
var destinationAirport = _airportRepository.GetByCode(selectedFlight.DestinationAirport.Code);
```

Remove `using SkyRoute.API.Data;` from `BookingService.cs`.

**DI registration in `Program.cs`:**

```csharp
builder.Services.AddSingleton<IAirportRepository, AirportRepository>();
```

---

### Step 3: `IFlightSearchService`

**Purpose:** Allow controller tests to mock flight search behavior without constructing the real service + providers + repository tree.

**New interface** — `src/SkyRoute.API/Services/IFlightSearchService.cs`:

```csharp
using SkyRoute.API.Contracts.Requests;
using SkyRoute.API.Contracts.Responses;

namespace SkyRoute.API.Services;

public interface IFlightSearchService
{
    IReadOnlyList<FlightSearchResponse> Search(FlightSearchRequest request);
}
```

**Changes to `FlightSearchService.cs`:**

```csharp
// Before: public sealed class FlightSearchService
// After:  public sealed class FlightSearchService : IFlightSearchService
```

**Changes to `FlightsController.cs`:**

```csharp
// Before:
private readonly FlightSearchService _flightSearchService;
public FlightsController(FlightSearchService flightSearchService)

// After:
private readonly IFlightSearchService _flightSearchService;
public FlightsController(IFlightSearchService flightSearchService)
```

**DI registration in `Program.cs`:**

```csharp
// Before:
builder.Services.AddScoped<FlightSearchService>();

// After:
builder.Services.AddScoped<IFlightSearchService, FlightSearchService>();
```

**Test impact — Before vs After:**

*Before (current `FlightsControllerTests.cs`):*
```csharp
public FlightsControllerTests()
{
    _providerMock = new Mock<IFlightProvider>();
    _providerMock.Setup(p => p.Search(It.IsAny<FlightSearchRequest>())).Returns([]);
    var repository = new FlightOfferRepository();
    var searchService = new FlightSearchService([_providerMock.Object], repository);
    _controller = new FlightsController(searchService);
}
```

*After:*
```csharp
public FlightsControllerTests()
{
    _searchServiceMock = new Mock<IFlightSearchService>();
    _controller = new FlightsController(_searchServiceMock.Object);
}
```

Each test sets up only its relevant behavior:

```csharp
[Fact]
public void SearchFlights_ServiceThrowsException_Returns500()
{
    var request = new FlightSearchRequest("EZE", "GRU", futureDate, 1, CabinClass.Economy);
    _searchServiceMock.Setup(s => s.Search(request)).Throws(new Exception("unexpected"));

    var result = _controller.SearchFlights(request);

    var statusResult = Assert.IsType<ObjectResult>(result);
    Assert.Equal(500, statusResult.StatusCode);
}
```

The `500` path is now testable — impossible before because you could never trigger a non-`ArgumentException` through the real service.

---

### Step 4: `IBookingService`

**Purpose:** Same as Step 3 — isolate controller tests from real booking logic.

**New interface** — `src/SkyRoute.API/Services/IBookingService.cs`:

```csharp
using SkyRoute.API.Contracts.Requests;
using SkyRoute.API.Models;

namespace SkyRoute.API.Services;

public interface IBookingService
{
    Booking ConfirmBooking(CreateBookingRequest request);
}
```

**Changes to `BookingService.cs`:**

```csharp
// Before: public sealed class BookingService
// After:  public sealed class BookingService : IBookingService
```

**Changes to `BookingController.cs`:**

```csharp
// Before:
private readonly BookingService _bookingService;
public BookingController(BookingService bookingService)

// After:
private readonly IBookingService _bookingService;
public BookingController(IBookingService bookingService)
```

**DI registration in `Program.cs`:**

```csharp
// Before:
builder.Services.AddScoped<BookingService>();

// After:
builder.Services.AddScoped<IBookingService, BookingService>();
```

**Test impact — Before vs After:**

*Before (current `BookingControllerTests.cs`):*
```csharp
public BookingControllerTests()
{
    _repository = new FlightOfferRepository();
    var bookingService = new BookingService(_repository);
    _controller = new BookingController(bookingService);
}
```

*After:*
```csharp
public BookingControllerTests()
{
    _bookingServiceMock = new Mock<IBookingService>();
    _controller = new BookingController(_bookingServiceMock.Object);
}
```

The `ExtractErrorMessage` reflection helper and the `CreateOffer` helper are no longer needed in controller tests.

---

## Complete DI registration after all steps

```csharp
// Program.cs — final state

using Scalar.AspNetCore;
using SkyRoute.API.Services;
using SkyRoute.API.Providers;
using SkyRoute.API.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Infrastructure — providers
builder.Services.AddScoped<IFlightProvider, BudgetWingsProvider>();
builder.Services.AddScoped<IFlightProvider, GlobalAirProvider>();

// Infrastructure — data
builder.Services.AddSingleton<IFlightOfferRepository, FlightOfferRepository>();
builder.Services.AddSingleton<IAirportRepository, AirportRepository>();

// Application — services
builder.Services.AddScoped<IFlightSearchService, FlightSearchService>();
builder.Services.AddScoped<IBookingService, BookingService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("SkyRoute Flights API")
            .WithTheme(ScalarTheme.DeepSpace)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

---

## Test rewrites

### FlightsControllerTests.cs (final)

```csharp
using Microsoft.AspNetCore.Mvc;
using Moq;
using SkyRoute.API.Contracts.Requests;
using SkyRoute.API.Contracts.Responses;
using SkyRoute.API.Controllers;
using SkyRoute.API.Models;
using SkyRoute.API.Services;

namespace SkyRoute.Test.Controllers;

[Trait("Category", "Unit")]
public sealed class FlightsControllerTests
{
    private readonly Mock<IFlightSearchService> _searchServiceMock;
    private readonly FlightsController _controller;

    public FlightsControllerTests()
    {
        _searchServiceMock = new Mock<IFlightSearchService>();
        _controller = new FlightsController(_searchServiceMock.Object);
    }

    [Fact]
    public void SearchFlights_PastDepartureDate_ReturnsBadRequest()
    {
        var request = new FlightSearchRequest(
            "EZE", "GRU",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            1, CabinClass.Economy);

        var result = _controller.SearchFlights(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Departure date cannot be in the past.", badRequest.Value);
    }

    [Fact]
    public void SearchFlights_SameOriginAndDestination_ReturnsBadRequest()
    {
        var request = new FlightSearchRequest(
            "EZE", "EZE",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            1, CabinClass.Economy);

        var result = _controller.SearchFlights(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Origin and destination airports cannot be the same.", badRequest.Value);
    }

    [Fact]
    public void SearchFlights_ValidRequest_ReturnsOkWithResults()
    {
        var request = new FlightSearchRequest(
            "EZE", "GRU",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            1, CabinClass.Economy);
        var expected = new List<FlightSearchResponse> { /* ... */ };
        _searchServiceMock.Setup(s => s.Search(request)).Returns(expected);

        var result = _controller.SearchFlights(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var results = Assert.IsAssignableFrom<IReadOnlyList<FlightSearchResponse>>(okResult.Value);
        Assert.Single(results);
    }

    [Fact]
    public void SearchFlights_ServiceThrowsException_Returns500()
    {
        var request = new FlightSearchRequest(
            "EZE", "GRU",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            1, CabinClass.Economy);
        _searchServiceMock.Setup(s => s.Search(request))
            .Throws(new Exception("unexpected"));

        var result = _controller.SearchFlights(request);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }
}
```

### BookingControllerTests.cs (final)

```csharp
using Microsoft.AspNetCore.Mvc;
using Moq;
using SkyRoute.API.Contracts.Requests;
using SkyRoute.API.Contracts.Responses;
using SkyRoute.API.Controllers;
using SkyRoute.API.Models;
using SkyRoute.API.Services;

namespace SkyRoute.Test.Controllers;

[Trait("Category", "Unit")]
public sealed class BookingControllerTests
{
    private readonly Mock<IBookingService> _bookingServiceMock;
    private readonly BookingController _controller;

    public BookingControllerTests()
    {
        _bookingServiceMock = new Mock<IBookingService>();
        _controller = new BookingController(_bookingServiceMock.Object);
    }

    [Fact]
    public void CreateBooking_ValidDomesticRequest_Returns201WithReferenceCode()
    {
        var request = new CreateBookingRequest(1, new List<CreatePassengerRequest>
        {
            new("John Doe", "john@test.com", DocumentType.NationalId, "12345678"),
        });
        var booking = new Booking
        {
            Id = 1, ReferenceCode = "SKY-ABC123", CreatedAtUtc = DateTime.UtcNow,
            ProviderName = "BudgetWings", FlightNumber = "BW101",
            OriginAirportCode = "EZE", DestinationAirportCode = "COR",
            DepartureTime = DateTime.UtcNow, ArrivalTime = DateTime.UtcNow,
            CabinClass = CabinClass.Economy, Passengers = [],
            PricePerPassenger = 100m, TotalPrice = 100m,
        };
        _bookingServiceMock.Setup(s => s.ConfirmBooking(request)).Returns(booking);

        var result = _controller.CreateBooking(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
        var response = Assert.IsType<CreateBookingResponse>(createdResult.Value);
        Assert.Equal("SKY-ABC123", response.BookingReferenceCode);
    }

    [Fact]
    public void CreateBooking_FlightNotFound_Returns400WithError()
    {
        var request = new CreateBookingRequest(999, []);
        _bookingServiceMock.Setup(s => s.ConfirmBooking(request))
            .Throws(new ArgumentException("Flight offer with ID 999 not found."));

        var result = _controller.CreateBooking(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("999", badRequest.Value!.ToString());
    }

    [Fact]
    public void CreateBooking_InternationalRouteWithNationalId_Returns400WithError()
    {
        var request = new CreateBookingRequest(1, new List<CreatePassengerRequest>
        {
            new("Jane Doe", "jane@test.com", DocumentType.NationalId, "12345678"),
        });
        _bookingServiceMock.Setup(s => s.ConfirmBooking(request))
            .Throws(new InvalidOperationException("must provide a Passport Number"));

        var result = _controller.CreateBooking(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Passport", badRequest.Value!.ToString());
    }

    [Fact]
    public void CreateBooking_UnexpectedException_Returns500()
    {
        var request = new CreateBookingRequest(1, new List<CreatePassengerRequest>
        {
            new("John Doe", "john@test.com", DocumentType.NationalId, "12345678"),
        });
        _bookingServiceMock.Setup(s => s.ConfirmBooking(request))
            .Throws(new Exception("unexpected"));

        var result = _controller.CreateBooking(request);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }
}
```

---

## Migration execution order

| # | Action | Files affected |
|---|---|---|
| 1 | Create `IFlightOfferRepository` | 1 new file |
| 2 | Update `FlightOfferRepository : IFlightOfferRepository` | 1 edit |
| 3 | Update `FlightSearchService` — use `IFlightOfferRepository` | 1 edit |
| 4 | Update `BookingService` — use `IFlightOfferRepository` + add `IAirportRepository` | 1 edit |
| 5 | Create `IAirportRepository` | 1 new file |
| 6 | Create `AirportRepository : IAirportRepository` | 1 new file |
| 7 | Create `IFlightSearchService` | 1 new file |
| 8 | Update `FlightSearchService : IFlightSearchService` | 1 edit |
| 9 | Create `IBookingService` | 1 new file |
| 10 | Update `BookingService : IBookingService` | 1 edit |
| 11 | Update `FlightsController` — use `IFlightSearchService` | 1 edit |
| 12 | Update `BookingController` — use `IBookingService` | 1 edit |
| 13 | Update `Program.cs` — new DI registrations, remove old ones | 1 edit |
| 14 | Rewrite `FlightsControllerTests.cs` — mock `IFlightSearchService` | 1 edit |
| 15 | Rewrite `BookingControllerTests.cs` — mock `IBookingService` | 1 edit |
| 16 | Update `BookingServiceTests.cs` — inject `IAirportRepository` mock | 1 edit |
| 17 | Update `FlightSearchServiceTests.cs` — use `IFlightOfferRepository` | optional, 0 edits if pattern works |
| 18 | Update `FlightOfferRepositoryTests.cs` — use interface or keep concrete | 0 edits, class satisfies interface |
| | **Verify** `dotnet build && dotnet test` | — |

Total: **5 new files**, **10 edited files**, **0 deleted files** (for this phase).

---

## Layer separation (future)

After all 4 interfaces are extracted and tests pass, the project-split phase becomes mechanical:

1. Create `SkyRoute.Domain` class library → move `Models/` there
2. Create `SkyRoute.Application` class library → move `Interfaces/`, `Services/`, `Contracts/`, `DTOs/` there
3. Create `SkyRoute.Infrastructure` class library → move `Data/`, `Providers/` there
4. Update project references in `.csproj` files
5. Update `using` namespaces
6. Move test project to reference `SkyRoute.API` (still the composition root)

Each new project gets its own `DependencyInjection.cs` with `Add{Layer}()` extension methods, keeping `Program.cs` clean.
