# Sudan Trains — repo guide

Monorepo for **قطارات السودان (Sudan Trains)**, a rail-booking platform. One .NET 8 backend, two React/Vite frontends. Both frontends are **Arabic-only, RTL**.

## Top-level layout

| Path | What it is |
| --- | --- |
| `apps/backend/` | .NET 8 API + service layers. Solution: `_Trains.sln`. |
| `apps/frontend/admin/` | Admin dashboard (manage trains, routes, fares, bookings). |
| `apps/frontend/customer/` | Public-facing booking site (search → seat → pay). |
| `docs/` | Repo-wide product + deployment docs. |
| `apps/backend/docs/` | Backend architecture, auth, DB, maps, localization. |

## Local dev ports

- Backend API: `8081` (set via `ASPNETCORE_URLS`)
- Customer app: `5173`
- Admin app: `3001`

Both frontends default `VITE_API_URL` to `http://localhost:8081`.

## Where to look first

- Backend-only task → [`apps/backend/CLAUDE.md`](apps/backend/CLAUDE.md)
- Admin UI task → [`apps/frontend/admin/CLAUDE.md`](apps/frontend/admin/CLAUDE.md)
- Customer UI task → [`apps/frontend/customer/CLAUDE.md`](apps/frontend/customer/CLAUDE.md)
- Cross-cutting (touches multiple apps) → start here, then drill in.
- Deeper background or design rationale → `docs/` or `apps/backend/docs/`.

## Conventions worth knowing repo-wide

- **Brand naming**: namespaces are `Sudan_Train.*`; csproj filenames use a legacy `Trains.*` prefix; folder names are `Sudan_Train.*`. The IDE will complain about the mismatch — ignore.
- **Arabic everywhere on the customer-visible surfaces**. Both frontends are Arabic-only; don't introduce English literals into JSX. Admin uses `AR.{group}.{key}`; customer uses `t('some.key')` via `LanguageContext`.
- **Domain primitives** that recur:
  - `CoachClass` — `First = 1`, `Second = 2`, `Third = 3`.
  - `BookingStatus` — `Pending | Confirmed | Cancelled | Completed` (PascalCase strings on the wire).
  - Fare resolution priority: **Trip > Segment > Route** (handled server-side; clients just call `getApplicableFare`).
  - Per-segment seat overlap: `[b1, a1]` and `[b2, a2]` overlap iff `b1 < a2 && b2 < a1` (stop orders). Implemented in `BookingService`/`TripService`.

## Planning

Multi-step plans for this repo live in `~/.claude/plans/` (outside the repo), not under `docs/`. The current plan file is at the user-specific path declared by plan mode — don't commit it here.
