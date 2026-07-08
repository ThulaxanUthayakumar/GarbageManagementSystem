<<<<<<< HEAD
# Garbage Management System

A full-stack **Garbage Management System** built as a portfolio project for a Junior / Associate .NET Developer interview. Residents can request waste collection, view collection schedules, and file complaints. Admins manage residents, schedules, requests, complaints, and view analytics.

**Backend:** ASP.NET Core 8 Web API · EF Core (Code-First) · SQL Server · ASP.NET Identity · JWT Auth · AutoMapper · FluentValidation · Swagger
**Frontend:** React 18 · Vite · TypeScript · MUI (Material UI) · Axios · React Router · React Hook Form · Recharts

> **A note on how this was built:** this project was generated in a sandboxed environment with no internet access and no .NET SDK installed, so the code was written carefully by hand but **could not be compiled or run here to verify it**. It follows standard, well-established ASP.NET Core 8 / EF Core / Identity / React patterns throughout, but please treat it as a strong first draft: run it locally following the steps below, and if you hit a build error, it's most likely a small package version mismatch or a typo that's easy to fix (see Troubleshooting at the bottom).

---

## 1. Folder structure

```
GarbageManagementSystem/
├── backend/
│   ├── GarbageManagementSystem.sln
│   └── GarbageManagementSystem.API/
│       ├── Controllers/        # API endpoints (Auth, Residents, Requests, Schedules, Complaints, Dashboard, Reports, WasteCategories)
│       ├── Models/
│       │   ├── Entities/       # EF Core entities (ApplicationUser, Resident, WasteCategory, CollectionRequest, CollectionSchedule, Complaint)
│       │   └── Enums/          # RequestStatus, ScheduleStatus, ComplaintStatus, UserRoles
│       ├── DTOs/                # Request/response models, grouped by feature
│       ├── Data/
│       │   ├── ApplicationDbContext.cs
│       │   └── Seed/DbSeeder.cs
│       ├── Repositories/        # Generic + entity-specific repositories
│       ├── Services/            # Business logic layer
│       ├── Validators/          # FluentValidation rules
│       ├── Mappings/            # AutoMapper profile
│       ├── Middleware/          # Global exception handling
│       ├── Filters/             # Automatic FluentValidation action filter
│       ├── Common/              # Custom exceptions (NotFoundException, etc.)
│       ├── Program.cs
│       └── appsettings.json
├── frontend/
│   └── src/
│       ├── api/                 # Axios calls, one file per module
│       ├── components/          # layout, common, and chart components
│       ├── context/              # AuthContext, ToastContext
│       ├── hooks/
│       ├── pages/                # auth, residents, collectionRequests, schedules, complaints, reports, profile
│       ├── theme/                 # MUI theme + light/dark mode
│       └── types/                 # Shared TypeScript types (mirrors the backend DTOs)
├── README.md
├── LICENSE
└── .gitignore
```

---

## 2. Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, a full instance, or SQL Server in Docker) — see section 4
- [Node.js 18+](https://nodejs.org/) and npm
- The EF Core CLI tool: `dotnet tool install --global dotnet-ef`

---

## 3. Running the backend

```bash
cd backend/GarbageManagementSystem.API

# 1. Restore NuGet packages
dotnet restore

# 2. Point appsettings.json at your SQL Server (see section 4 if you need to change it)

# 3. Generate the initial migration (only needed once, the first time you set this up)
dotnet ef migrations add InitialCreate

# 4. Run the API — this automatically applies the migration and seeds demo data
dotnet run
```

The API starts on **http://localhost:5001** (and https://localhost:7001) and opens Swagger UI at `/swagger` automatically in Development mode.

> Steps 3–4 only need to happen once. On every subsequent `dotnet run`, `Program.cs` calls `Database.MigrateAsync()` and `DbSeeder.SeedAsync()` automatically, so your schema and demo data stay up to date.

### Seeded accounts (created automatically the first time you run the app)

| Role     | Email                     | Password      |
|----------|---------------------------|---------------|
| Admin    | `[email protected]`     | `Admin@123`   |
| Resident | `[email protected]`  | `Resident@123`|
| Resident | `[email protected]`  | `Resident@123`|

(A few more sample residents, requests, schedules and complaints are seeded too, spread across "Zone A", "Zone B" and "Zone C", so the dashboard and reports have something to show immediately.)

---

## 4. Database setup

The default connection string in `appsettings.json` targets a local SQL Server instance:

```json
"DefaultConnection": "Server=localhost;Database=GarbageManagementDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

Depending on your setup, you may need to change this:

- **SQL Server LocalDB (Windows):** `Server=(localdb)\\mssqllocaldb;Database=GarbageManagementDb;Trusted_Connection=True;TrustServerCertificate=True`
- **SQL Server in Docker:** `Server=localhost,1433;Database=GarbageManagementDb;User Id=sa;Password=YourPassword1!;TrustServerCertificate=True`
- **A named instance or remote server:** update `Server=` accordingly, and use SQL auth (`User Id=`/`Password=`) if you're not using Windows/Trusted auth.

After changing the connection string, re-run `dotnet ef database update` (or just `dotnet run`, which does it for you).

---

## 5. Running the frontend

```bash
cd frontend
npm install
cp .env.example .env   # already included as .env with sensible defaults — edit VITE_API_URL if your API runs on a different port
npm run dev
```

The app starts on **http://localhost:5173**. It's already configured to talk to the backend at `http://localhost:5001/api` — update `frontend/.env` if you changed the backend's port.

---

## 6. API endpoints

All responses are wrapped in a consistent envelope: `{ success, message, data, errors }`.

### Auth (`/api/auth`)
| Method | Endpoint | Access | Description |
|---|---|---|---|
| POST | `/register` | Public | Register a new resident account |
| POST | `/login` | Public | Log in and receive a JWT |
| POST | `/logout` | Authenticated | Client-side token cleanup (JWTs are stateless) |
| POST | `/change-password` | Authenticated | Change your own password |
| GET | `/me` | Authenticated | Get the current user's profile |

### Residents (`/api/residents`) — Admin only
| Method | Endpoint | Description |
|---|---|---|
| GET | `/?pageNumber=&pageSize=&searchTerm=` | Paged, searchable list |
| GET | `/{id}` | Get one resident |
| POST | `/` | Create a resident (+ login account) |
| PUT | `/{id}` | Update a resident |
| DELETE | `/{id}` | Delete a resident |

### Collection Requests (`/api/collectionrequests`)
| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/?pageNumber=&pageSize=&searchTerm=&status=` | Admin | Paged, filterable list of all requests |
| GET | `/my` | Resident | The caller's own requests |
| GET | `/{id}` | Both (owner-checked) | Get one request |
| POST | `/` (multipart/form-data) | Resident | Submit a request, optionally with a photo |
| PUT | `/{id}/status` | Admin | Update status (Pending / InProgress / Completed / Cancelled) |
| DELETE | `/{id}` | Admin | Delete a request |

### Schedules (`/api/schedules`)
| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/?pageNumber=&pageSize=&zone=` | Admin | Paged list, filterable by zone |
| GET | `/my` | Resident | Schedule entries for the caller's own zone |
| GET | `/{id}` | Both | Get one schedule entry |
| POST | `/` | Admin | Create a schedule entry |
| PUT | `/{id}` | Admin | Update a schedule entry |
| DELETE | `/{id}` | Admin | Delete a schedule entry |

### Complaints (`/api/complaints`)
| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/?pageNumber=&pageSize=&searchTerm=&status=` | Admin | Paged, filterable list of all complaints |
| GET | `/my` | Resident | The caller's own complaints |
| GET | `/{id}` | Both (owner-checked) | Get one complaint |
| POST | `/` (multipart/form-data) | Resident | Submit a complaint, optionally with a photo |
| PUT | `/{id}/status` | Admin | Update status + admin remarks |
| DELETE | `/{id}` | Admin | Delete a complaint |

### Dashboard (`/api/dashboard`)
| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/stats` | Admin | Top-line counters |
| GET | `/resident-summary` | Resident | Personal counters |
| GET | `/collection-trend?months=` | Admin | Monthly trend for charts |
| GET | `/waste-category-breakdown` | Admin | Request counts by category |

### Reports (`/api/reports`) — Admin only
| Method | Endpoint | Description |
|---|---|---|
| GET | `/daily-collections?date=` | Totals for a single day |
| GET | `/monthly-collections?year=&month=` | Totals + day-by-day breakdown |
| GET | `/complaint-statistics` | Complaint counts by status + avg. resolution time |
| GET | `/waste-category-statistics` | Request counts grouped by category |

### Waste Categories (`/api/wastecategories`)
| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/` | Authenticated | List all categories (for dropdowns) |
| GET | `/{id}` | Authenticated | Get one category |
| POST / PUT / DELETE | `/` | Admin | Manage categories |

Full interactive documentation (with a "Try it out" button and JWT bearer auth support) is available at **`/swagger`** once the backend is running.

---

## 7. Screenshots

_Add screenshots here once you've run the app locally — a couple of good ones to include:_

- `docs/screenshots/dashboard-admin.png` — Admin dashboard with stat cards and charts
- `docs/screenshots/dashboard-resident.png` — Resident dashboard view
- `docs/screenshots/residents.png` — Resident management table
- `docs/screenshots/requests.png` — Collection requests (with photo upload)
- `docs/screenshots/schedule.png` — Collection schedule
- `docs/screenshots/complaints.png` — Complaints management
- `docs/screenshots/reports.png` — Reports & charts
- `docs/screenshots/swagger.png` — Swagger UI

```markdown
![Admin Dashboard](docs/screenshots/dashboard-admin.png)
```

---

## 8. Troubleshooting

- **"Pending model changes" or migration errors on first run** — make sure you ran `dotnet ef migrations add InitialCreate` *before* your first `dotnet run`.
- **Cannot connect to SQL Server** — double check the connection string in `appsettings.json` matches how SQL Server is actually running on your machine (see section 4).
- **CORS errors in the browser console** — confirm the frontend's `VITE_API_URL` and the backend's `Cors:AllowedOrigins` (in `appsettings.json`) agree on ports.
- **`dotnet ef` command not found** — run `dotnet tool install --global dotnet-ef`, then restart your terminal.
- **Images (photos on requests/complaints) don't show up** — they're served from the backend's `wwwroot/uploads` folder; make sure the backend is running and reachable at the URL in `VITE_API_URL`.
- **A NuGet or npm package version conflict** — the versions pinned in `GarbageManagementSystem.API.csproj` and `frontend/package.json` were current as of when this was generated; bumping a package to the latest minor/patch version is usually safe if you hit a resolution error.

---

## 9. Tech decisions worth knowing for the interview

- **Single Web API project**, organized by folder (Controllers/Models/DTOs/Services/Repositories/Data) rather than split across multiple class-library projects — easier to navigate for a project this size.
- **Repository + Service layering**: controllers never touch `DbContext` directly; repositories handle data access, services hold business logic and orchestration (e.g. creating an Identity user *and* a Resident profile together).
- **FluentValidation via a global action filter** rather than the deprecated `FluentValidation.AspNetCore` package — validators are discovered automatically and run before the controller action executes.
- **JWT auth is stateless** — `/api/auth/logout` exists for completeness, but real "logout" is the client discarding its token; there's no server-side token blacklist (a reasonable simplification for a project this size).
- **Enums serialize as strings** (e.g. `"Pending"` not `0`) for a much more readable API and frontend.

---

## License

MIT — see [LICENSE](LICENSE).
=======
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
>>>>>>> 787ac9f7e7fcec5ad329452ea29ac1126972c963
