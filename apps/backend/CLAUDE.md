# Sudan Trains — backend guide

.NET 8 + EF Core 8 + MediatR. Solution file: `_Trains.sln`. Runs on port `8081`.

## Solution layout

| csproj (filename) | Folder | Role |
| --- | --- | --- |
| `Trains.Api.csproj` | `Sudan_Train/` | API host (controllers, `Program.cs`, JWT setup). |
| `Trains.Core.csproj` | `Sudan_Train.Core/` | CQRS features, middleware, `Response<T>` envelope. |
| `Trains.Service.csproj` | `Sudan_Train.Service/` | Business logic + service abstracts. |
| `Trains.Data.csproj` | `Sudan_Train.Data/` | Entities, DTOs, enums, `AppMetaData/`. |
| `Trains.Infrastructure.csproj` | `Sudan_Train.Infrastructure/` | `ApplicationDBContext`, EF configurations, migrations, repositories, seeder. |
| `Sudan_Train.MessagingApi.csproj` | `Sudan_Train.MessagingApi/` | Email service (small, single-purpose). |

**Naming quirk**: csproj files use a legacy `Trains.*` prefix, but folder + namespace are `Sudan_Train.*`. The IDE may flag namespace mismatch — ignore.

## CQRS feature pattern

Every command/query follows the same triplet inside `Sudan_Train.Core/Features/{Feature}/`:

```
Commands/{Op}/{Op}Command.cs           // : IRequest<Response<TDto>>
Commands/{Op}/{Op}CommandValidator.cs  // : AbstractValidator<{Op}Command>  (FluentValidation)
Commands/{Op}/{Op}CommandHandler.cs    // : ResponseHandler, IRequestHandler<{Op}Command, Response<TDto>>
Queries/{Q}/...                        // same trio shape
```

Canonical examples to copy from:
- [`Features/Bookings/`](Sudan_Train.Core/Features/Bookings/) — transactional create with race re-check.
- [`Features/Infrastructure/Fares/`](Sudan_Train.Core/Features/Infrastructure/Fares/) — polymorphic-scope CRUD.

Register handlers via MediatR auto-discovery in `Program.cs`. Register services via `ModuleServiceDependencies.cs` in `Sudan_Train.Service`.

## Response envelope

Every handler returns `Response<T>` from `Sudan_Train.Core/Bases`. Use the `ResponseHandler` helpers — **don't return raw status codes**:

```csharp
return Success(messageKey, data);
return Created(messageKey, data);
return NotFound<T>(message);
return BadRequest<T>(message);
return UnprocessableEntity<T>(message);   // 422 — use this for seat-taken conflicts (router base doesn't map 409)
return Unauthorized<T>(message);
```

Controllers just `await _mediator.Send(...)` and `return Ok(response)`.

## Service layer

- `IFooService` lives in `Sudan_Train.Service/Abstracts/`, implementation in `Sudan_Train.Service/Implementations/`.
- Handlers stay thin; **business logic and EF queries live in the service**.
- When a service method takes many params, define a `FooOpInput` POCO in the same file as the interface (see `BookingService` / `CreateBookingInput`).
- Map entities → DTOs in a `private static MapToDto(Foo f)` helper at the bottom of the service.

## DTOs

`Sudan_Train.Data/DTOs/{Group}/`. Naming `FooDto`. Customer-facing surfaces (e.g. `BookingDto`) live under `DTOs/Booking/`; admin/infra surfaces under `DTOs/Infrastructure/`.

## Entities + enums

- Entities in `Sudan_Train.Data/Entity/`. Audit fields (CreatedAt/UpdatedAt) come from `AuditableEntity`.
- All enums centralised in [`Sudan_Train.Data/Entity/Enums.cs`](Sudan_Train.Data/Entity/Enums.cs). Don't define enums next to entities.
- EF configurations live in `Sudan_Train.Infrastructure/Configurations/{Foo}Configuration.cs`, one per entity.

## EF Core + migrations

Commands (always with both `--project` and `--startup-project`):

```bash
dotnet ef migrations add {Name} --project Sudan_Train.Infrastructure --startup-project Sudan_Train
dotnet ef database update      --project Sudan_Train.Infrastructure --startup-project Sudan_Train
dotnet ef migrations remove    --project Sudan_Train.Infrastructure --startup-project Sudan_Train
```

**Decimal column types** must be explicit in the corresponding `Configuration.cs` — defaults truncate silently:

- Prices / monetary: `HasColumnType("decimal(18,2)")`
- Percent fields: `HasColumnType("decimal(5,2)")`

## Auth

- JWT bearer. Roles in `Sudan_Train.Data/AppMetaData/Roles.cs`: `SuperAdmin`, `Admin`, `Staff`, `Customer`, `User`. Convenience constant `Roles.AdminOrStaff`.
- Admin endpoints: `[Authorize(Roles = Roles.AdminOrStaff)]`. Public read endpoints: `[AllowAnonymous]`. Customer-owned actions: `[Authorize]` and pull `userId` from `ClaimTypes.NameIdentifier` (or fallback `"uid"`).

## Booking-flow invariants (cross-cutting)

- **Per-segment seat inventory**: `BookingPassenger` carries `BoardingStationId` + `AlightingStationId`. Two passengers can share a seat on non-overlapping legs.
- **Overlap check** (server-side): `[b1, a1]` and `[b2, a2]` overlap iff `b1 < a2 && b2 < a1` (stop orders; origin = 0, destination = max+1, intermediates from `RouteStation.StopOrder`).
- **Race re-check**: `BookingService.CreateBookingAsync` re-runs the overlap query inside a transaction. Conflict → `UnprocessableEntity` (422).
- **Fare resolution** (Trip > Segment > Route): [`FareService.GetApplicableFareAsync`](Sudan_Train.Service/Implementations/FareService.cs).
- **Fare auto-close**: `FareService.CreateFareAsync` closes any active fare with the exact same `(RouteId, OriginStationId, DestinationStationId, TripId, CoachClass)` tuple by setting its `EffectiveTo = now` before inserting the new row.

## Build

```bash
dotnet build _Trains.sln
```

Warnings around vulnerability advisories on `MimeKit`/`MailKit`, async-without-await on auth, and `IDE0130` namespace style are pre-existing — don't try to "fix" them as part of an unrelated change.

## Deeper reading

[`apps/backend/docs/`](docs/) covers architecture, authentication, configuration, database, localization, and maps in depth. Use that for design rationale; this file is just the minimum to navigate.
