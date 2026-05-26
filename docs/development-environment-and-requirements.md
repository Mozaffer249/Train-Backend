# Sudan Train Platform — Development Environment & Requirements

Simple reference for **backend**, **frontend**, deployment assumptions, and how the system is put together.

---

## Development environment

### Backend

- **Stack:** .NET 8, ASP.NET Core Web API.
- **Local:** Install the **.NET 8 SDK**, restore/build the solution (`_Trains.sln` under `apps/backend`), configure connection strings and `AppSettings`, run EF migrations if needed, then run the main API and optionally the Messaging API.
- **Containers:** From the monorepo root, **Docker Compose** can start SQL Server, RabbitMQ, the main API, Messaging API, and related services together.

### Frontend

Two **React** single-page apps live under `apps/frontend/`:

| App | Purpose | Dev URL (typical) | Docker URL (typical) |
|-----|---------|-------------------|----------------------|
| **Customer** (`customer/`) | Public booking, auth, tickets | http://localhost:5173 | http://localhost:3000 |
| **Admin** (`admin/`) | Staff/admin: users, bookings, fleet, trips | http://localhost:3001 | http://localhost:3001 |

- **Local:** `cd` into `customer` or `admin`, run `npm install`, then `npm run dev`.
- **Env:** Use `.env.local` with `VITE_API_URL` and `VITE_MESSAGING_API_URL` pointing at your running APIs (e.g. main API on port 8080, Messaging API on 5001 when not using Docker port mapping differences).

### Full stack

- Run **SQL Server** (or use the Compose service), **RabbitMQ** if you use messaging features, **backend APIs**, then one or both frontends—or use **`docker-compose up`** for an integrated setup per your deployment docs.

---

## Hardware requirements

Practical **minimums for local development** (adjust upward for production):

| Resource | Suggestion |
|----------|------------|
| **CPU** | Multi-core (4+ cores is comfortable for IDE + Docker + DB) |
| **RAM** | **8 GB** minimum; **16 GB** recommended if Docker, SQL Server, and browser dev tools run together |
| **Disk** | **SSD** with **20+ GB** free for SDKs, Docker images, `node_modules`, NuGet cache, and database files |
| **Network** | Internet for package restore (NuGet, npm) and for testing email/SMS/push integrations |

**Frontend note:** Running **two** Vite dev servers plus a browser adds memory; prefer **16 GB** when developing customer + admin + backend locally.

---

## Software requirements

### Required / common

| Software | Notes |
|----------|--------|
| **Windows 10/11** (or Linux/macOS) | Matches typical Visual Studio 2022 workflows on Windows |
| **.NET SDK 8.0** | Backend targets `net8.0` |
| **Node.js 18+** | Frontend prerequisite |
| **npm** (or yarn) | Frontend package management |
| **Git** | Version control |

### Backend & data

| Software | Notes |
|----------|--------|
| **SQL Server 2022** (or Azure SQL / compatible) | Primary database; Compose uses official SQL Server image |
| **Docker + Docker Compose** | Recommended for full stack |
| **RabbitMQ** | Message broker for async email/SMS/push flows |

### Optional integrations

| Service | Used for |
|---------|----------|
| SMTP / Mail | Email (e.g. via Messaging API) |
| Twilio | SMS |
| Firebase | Push notifications |

### IDE / editors

- **Visual Studio 2022** or **VS Code** / **JetBrains Rider** for C#.
- Any editor with **TypeScript/ESLint** support for the React apps.

---

## Programming languages and tools

### Languages

- **C#** — backend APIs, services, data layer.
- **TypeScript** — both frontend apps.
- **HTML/CSS** — via React and **Tailwind CSS**.

### Backend tooling

| Area | Technology |
|------|------------|
| Web API | ASP.NET Core, OpenAPI/Swagger (Swashbuckle) |
| Data | Entity Framework Core 8, SQL Server provider |
| Auth | ASP.NET Core Identity, JWT Bearer |
| Patterns | MediatR, FluentValidation |
| Messaging | RabbitMQ; MailKit/MimeKit; Twilio; Firebase Admin |
| Logging | Serilog |

### Frontend tooling

| Area | Technology |
|------|------------|
| UI | React 18, React DOM |
| Build / dev server | Vite 5 |
| Routing | React Router 7 |
| Styling | Tailwind CSS 3, Postcss, Autoprefixer |
| Icons | Lucide React |
| Lint | ESLint 9, typescript-eslint |
| Admin extras | Google Maps React API, Turf, SweetAlert2 |

### Shared / ops tools

- **dotnet CLI**, **EF Core tools** (migrations).
- **Postman** (or similar) for API testing.
- **nginx** (in Docker) for serving built static frontend assets in deployment setups.

---

## System implementation

### Backend (high level)

Layered **train booking** backend:

- **Trains.Core** — core/domain concepts.
- **Trains.Data** — EF Core, Identity, validation-related wiring.
- **Trains.Infrastructure** — database access, JWT, Swagger, SQL Server.
- **Trains.Service** — application services, caching, messaging, email helpers, OTP/QR where used.
- **Trains.Api** — main **REST API** (localization, logging, Swagger).
- **Sudan_Train.MessagingApi** — **microservice** for queued **email, SMS, and push**, using SQL Server, RabbitMQ, and external providers.

**Flow:** Clients call the **main API**; **JWT** and **Identity** handle auth; **MediatR** and **FluentValidation** support request handling and validation; **EF Core** persists to **SQL Server**; notification work can be offloaded via **RabbitMQ** to the **Messaging API**.

### Frontend (high level)

Two **SPA** clients share the same backend:

- **Customer app** — search, book, register/login, booking history, tickets; responsive layout.
- **Admin app** — user/booking/fleet/trip management, dashboard, **role-based** access (Admin/Staff).

Both apps:

- Talk to the **same backend API** (`VITE_API_URL`); messaging features may use **`VITE_MESSAGING_API_URL`** where applicable.
- Rely on **CORS** being allowed on the backend for their dev and deployed origins.
- Build to static assets (`npm run build` → `dist/`) for production; Docker/`nginx` can serve them per compose configuration.

### Monorepo layout (conceptual)

```
apps/
├── backend/     # .NET solution: API, Messaging API, Core, Data, Infrastructure, Service
└── frontend/
    ├── customer/  # Public site (Vite + React + TS)
    └── admin/     # Admin dashboard (Vite + React + TS)
```

---

## Quick reference URLs

| Component | Typical access |
|-----------|----------------|
| Main API (Swagger) | http://localhost:8080/swagger |
| Messaging API | http://localhost:5001 |
| RabbitMQ management | http://localhost:15672 |
| SQL Server | localhost:1433 |
| Customer (dev) | http://localhost:5173 |
| Admin (dev) | http://localhost:3001 |

*(Ports may differ based on `docker-compose` or local overrides—use your actual `.env` and Compose file.)*

---

## Related documentation

- [Platform docs index](./README.md)
- [Backend README](../apps/backend/README.md)
- [Frontend README](../apps/frontend/README.md)
- [Deployment](./deployment/README.md)

