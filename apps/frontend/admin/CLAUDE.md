# Sudan Trains — admin app guide

React 18 + TypeScript + Vite + Tailwind. Arabic-only, RTL. Runs on port `3001`. Talks to the backend at `VITE_API_URL` (default `http://localhost:8081`).

## Language: Arabic-only, RTL

- **Every visible string** goes through [`src/i18n/ar.ts`](src/i18n/ar.ts) as `AR.{group}.{key}` (e.g. `AR.fares.editFare`, `AR.common.save`). **Don't hard-code Arabic text in JSX** — add the key to `ar.ts` and reference it.
- Use **logical CSS properties** (`start-3`, `ms-0 me-3`, `text-start`, `ps-10 pe-4`) — not `left-3` / `mr-3` / `text-left`. The latter break under RTL.

## Auth

- JWT stored in `localStorage['admin_token']`. `services/api.ts` injects `Authorization: Bearer …` automatically and redirects to `/login` on a 401.
- Allowed roles for the admin app: `SuperAdmin`, `Admin`, `Staff`.

## API services

[`src/services/api.ts`](src/services/api.ts) exports one object per backend resource. Each exposes the usual `getAll` / `getById` / `create` / `update` / `delete` (where applicable):

`citiesApi`, `stationsApi`, `spatialApi`, `boundaryApi`, `routesApi`, `faresApi`, `trainsApi`, `tripsApi`, `bookingsApi`.

Add new resources here, **not in components**. Components call `await fooApi.method(...)` and handle UI state.

The `api` low-level helper wraps `fetchWithAuth` and unwraps the backend `Response<T>` envelope — calling code receives just `data` (and an exception on `succeeded: false`).

## DTO types

- [`src/types/infrastructure.ts`](src/types/infrastructure.ts) — most resources.
- [`src/types/geography.ts`](src/types/geography.ts) — City / Station / boundary shapes.

Types mirror backend DTOs in camelCase. Discriminated string enums (e.g. `BookingStatus`) use PascalCase strings on the wire (`'Pending'`, `'Confirmed'`, …) — match the server.

## Alert + confirm helpers

Use the SweetAlert wrappers from [`src/utils/alerts.ts`](src/utils/alerts.ts) — **don't `window.confirm`**:

```ts
const ok = await showConfirm(title, message, confirmText?);   // boolean
await showSuccess(title);
showError(title, extractErrorMessage(err));
```

`extractErrorMessage(err)` normalises strings out of `unknown` / `Error` / API-error-shapes.

## Modal pattern

Page owns the open/close + edit-target state:

```tsx
const [modalOpen, setModalOpen] = useState(false);
const [editing, setEditing] = useState<Foo | null>(null);
…
<FooModal isOpen={modalOpen} editFoo={editing}
  onClose={() => { setModalOpen(false); setEditing(null); }}
  onSuccess={() => { setModalOpen(false); setEditing(null); load(); }} />
```

For a worked example with edit-mode lock + optional `pinnedXxx` override, see [`components/fares/FareModal.tsx`](src/components/fares/FareModal.tsx) — `isEdit` and `pinnedTrip` flow into a combined `scopeLocked` flag.

## Tailwind classes worth knowing

- `admin-button` — primary CTA (Sudan-red).
- `admin-button-secondary` — outlined alternative.
- `admin-card` — panel wrapper with white bg + shadow.
- `admin-primary-{50…900}` — brand red scale.
- `sudan-gold-{50…900}`, `sudan-green-{50…900}` — flag accents (used for badges/chips).

## Pages + components

- Pages in [`src/pages/`](src/pages/) — one per route (`Dashboard`, `GeographyPage`, `RoutesPage`, `FaresPage`, `TrainsPage`, `TripsPage`, `BookingsPage`, `UsersPage`, `SeedingPage`, `Login`).
- Reusable bits in `src/components/{group}/` (`common/`, `fares/`, `trains/`, `trips/`, `routes/`, `geography/`, `layout/`, `map/`).
- `common/Pagination.tsx`, `common/FilterDropdown.tsx`, `common/StatusBadge.tsx` are the shared building blocks — reuse before rolling your own.

## Build

```bash
npm run build                              # tsc -b && vite build
./node_modules/.bin/tsc --noEmit           # type-check only (faster)
```

The Vite bundle warns about a ~570 kB main chunk — pre-existing, not something to "fix" unless asked.
