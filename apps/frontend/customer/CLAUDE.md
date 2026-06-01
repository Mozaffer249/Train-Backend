# Sudan Trains — customer app guide

React 18 + TypeScript + Vite + Tailwind. Arabic-only, RTL. Runs on port `5173`. Talks to the backend at `VITE_API_URL` (default `http://localhost:8081`).

## Language: Arabic-only, RTL

- Every visible string goes through [`src/contexts/LanguageContext.tsx`](src/contexts/LanguageContext.tsx). In a component:

  ```tsx
  const { t } = useLanguage();
  …
  <span>{t('booking.confirmed')}</span>
  ```

- **Add new keys to `LanguageContext.tsx`** — don't hard-code Arabic in JSX.
- Use **logical CSS properties** (`ml-2 rtl:ml-0 rtl:mr-2`, `start-3`, `text-start`) — not bare `mr-2` / `left-3`. RTL would break.

## Auth

- Customer JWT stored in localStorage via [`src/contexts/AuthContext`](src/contexts/AuthContext.tsx).
- Flow: register → confirm-email (code) → login → bearer attached on subsequent requests.
- Routes like `/book` and `/dashboard` are guarded by `ProtectedRoute`.

## API services

- [`src/services/api.ts`](src/services/api.ts)
  - `authApi` — register / confirmEmail / login / sendResetCode / resetPassword / logout.
  - `catalogApi` — **public read** endpoints: `getStations`, `getRoutes`, `getTrips`, `getTripById`, `getFares`, `getSegmentSeats`, `getApplicableFare`.
- [`src/services/bookingApi.ts`](src/services/bookingApi.ts)
  - `createBooking` (POST `/Bookings`)
  - `getMyBookings` (GET `/Bookings/Mine`)
  - `getById` (GET `/Bookings/{id}`)
  - `cancelBooking` (POST `/Bookings/{id}/Cancel`)

The `api` helper wraps `fetch`, unwraps the backend `Response<T>` envelope, and throws on `succeeded: false`.

## DTO types

[`src/types/api.ts`](src/types/api.ts) mirrors backend DTOs in camelCase. Notable:

- `BookingStatus = 'Pending' | 'Confirmed' | 'Cancelled' | 'Completed'` (PascalCase strings from the server).
- `SegmentSeatsDto` → `coaches: CoachSeatsDto[] → seats: AvailableSeatDto[]` (with `isAvailable` per seat for the requested leg).
- `FareDto` + `FareBreakdownDto` (`basePrice → discount → VAT → total`). `breakdown` is populated by `getApplicableFare`.

## Booking flow (the heart of the app)

`SearchResults` → `BookingPage` → `Dashboard` / e-ticket. Step-by-step:

1. **Search**: `SearchResults` fetches routes for the chosen origin+destination, then for each trip on those routes calls `catalogApi.getApplicableFare(tripId, boardingId, alightingId, coachClassId)`. Backend handles the Trip > Segment > Route priority. The card shows the resolved total and a gold scope chip ("سعر خاص بهذه الرحلة" / "سعر خاص بهذا المقطع") when the fare has `tripId` or `originStationId+destinationStationId` set.

2. **Passenger info** (`BookingPage` step 1): collects name, ID, birthDate, contact info. Triggers a background `getApplicableFare` refetch so the summary panel shows the up-to-date breakdown.

3. **Seat selection** (step 2): `catalogApi.getSegmentSeats(tripId, boardingId, alightingId)` returns the seat grid grouped by coach. **Per-segment overlap is server-side** — the UI just trusts each seat's `isAvailable` flag.

4. **Payment** (step 3): `bookingApi.createBooking(...)`. **Handle 422 ("seat just got taken") by re-fetching the seat map and bouncing back to step 2** — this is the race-condition guard. Other errors surface as the inline error banner.

5. **Confirmation** (step 4): renders `booking.bookingRef` plus the server-issued QR via `react-qr-code` (payload is `booking.ticket.qrPayload`).

## Dashboard

`bookingApi.getMyBookings()` lists the user's bookings. Tabs split by `isUpcoming` (status not Cancelled/Completed AND `departureTime > now`). The e-ticket modal shows the segment + breakdown + server QR.

## Branding

- Sudan flag palette: `sudan-red-{50…900}`, `sudan-green-{50…900}`, `sudan-gold-{50…900}`, `sudan-sand-{50…900}`. The booking flow uses `sudan-green` for the primary CTA.
- `react-qr-code` for tickets, fg color `#064e2a` (deep sudan-green).

## Build

```bash
npm run build                  # tsc -b && vite build
./node_modules/.bin/tsc -b     # type-check only
```
