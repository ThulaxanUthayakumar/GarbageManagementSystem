# Garbage Management System (GMS)

A production-grade municipal garbage management platform: residents report uncollected garbage, municipal admins approve and assign, collectors execute routes and upload GPS-tagged proof, and super admins govern the whole system.

## Tech Stack

- **Backend:** ASP.NET Core 8 Web API, C#, EF Core 8 (Code First), SQL Server, ASP.NET Identity, JWT + refresh tokens, AutoMapper, FluentValidation, Repository + Unit of Work, Serilog, Swagger, global exception handling middleware
- **Frontend:** React (Vite), Bootstrap 5, Axios, React Router, protected routes
- **Architecture:** Clean Architecture (Domain → Application → Infrastructure → API), SOLID

## Solution Structure

```
GarbageManagementSystem/
├── GarbageManagementSystem.sln
├── Backend/
│   ├── GMS.Domain/          # Entities, enums, constants, base contracts — zero framework coupling*
│   ├── GMS.Application/     # Use cases, DTOs, service interfaces, validation (Phase 3+)
│   ├── GMS.Infrastructure/  # EF Core DbContext, repositories, Identity, external services (Phase 2+)
│   └── GMS.API/             # Controllers, middleware, composition root
├── Frontend/                # React app (Phase 7)
└── docs/
    └── DATABASE-DESIGN.md   # ER diagram, relationships, keys, conventions
```

\* Exception: `GMS.Domain` references `Microsoft.Extensions.Identity.Stores` so `ApplicationUser`/`ApplicationRole` can live beside the entities that navigate to them. This is a deliberate, documented trade-off — see `docs/DATABASE-DESIGN.md`.

## Phase Roadmap

| Phase | Scope | Status |
|-------|-------|--------|
| 1 | Solution structure, clean architecture, database design, entity classes | ✅ Done |
| 2 | DbContext, Identity, JWT auth + refresh tokens, migrations, seeding | ⬜ |
| 3 | Repository + Unit of Work, services, CRUD APIs (pagination/filter/sort/search) | ⬜ |
| 4 | Complaint module (workflow, images, comments, status history) | ⬜ |
| 5 | Collection schedule module (routes, daily tasks, GPS proof) | ⬜ |
| 6 | Dashboard + reports APIs, notifications | ⬜ |
| 7 | React frontend (admin dashboard, role-based UI) | ⬜ |
| 8 | Deployment | ⬜ |

## Running Phase 1

Prerequisites: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (`dotnet --version` → 8.x). SQL Server is **not** needed until Phase 2.

```bash
cd GarbageManagementSystem
dotnet restore
dotnet build

cd Backend/GMS.API
dotnet run --launch-profile https
```

Then open:

- Swagger UI → https://localhost:7150/swagger
- Health check → https://localhost:7150/health

You should see structured Serilog output in the console and a rolling log file under `Backend/GMS.API/Logs/`.

> If your IDE ever complains about the hand-written `.sln`, regenerate it with:
> `dotnet new sln -n GarbageManagementSystem && dotnet sln add Backend/GMS.Domain Backend/GMS.Application Backend/GMS.Infrastructure Backend/GMS.API`

## Conventions

- All timestamps are stored in **UTC** and suffixed `Utc` (`CreatedAtUtc`).
- Every entity inherits `BaseEntity`: int PK, audit columns, **soft delete** (`IsDeleted`) enforced via global query filters (Phase 2).
- Role names are constants in `GMS.Domain.Constants.AppRoles` — never hardcode role strings.
- `DateOnly`/`TimeOnly` are used for schedule dates/times (EF Core 8 maps them natively to SQL Server `date`/`time`).
