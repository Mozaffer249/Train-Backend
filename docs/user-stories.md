# Sudan Trains — User Stories per Role

A page-by-page walkthrough of what each role can do, plus the exact list of
screenshots to capture for the PPT.

Five roles: **SuperAdmin**, **Admin**, **StaffCounter**, **StaffBoarding**,
**Customer**.

Each role section contains:

1. Persona (who this user is in the real world).
2. The pages they can reach.
3. User stories ("As a X, I want to Y, so that Z").
4. 📸 Screenshot manifest — the exact filenames to capture or reuse.

> **How to use this doc for the PPT.** Capture every screen listed in each
> role's manifest and save them under `docs/screens/<role>/<filename>.png`.
> Then upload this markdown plus the `screens/` folder to claude.ai and use
> the prompt at the bottom of this file.

---

## 1. SuperAdmin

### Persona
The platform owner / system operator. Same powers as Admin **plus** access to
the seeding/data-import tools used during initial setup and dev environments.

### Pages reachable

| URL | Page | Notes |
| --- | --- | --- |
| `/dashboard` | Dashboard | High-level KPIs (bookings, revenue, active trips). |
| `/trips` | Trips management | Full CRUD on scheduled trips. |
| `/bookings` | Bookings list | All bookings across the system. |
| `/counter` | Counter sale | Can sell tickets like a counter agent (override). |
| `/boarding` | Boarding portal | Can run any station's manifest (override). |
| `/refunds` | Refunds | Approve / reject refund requests. |
| `/users` | Users management | Create users, assign roles + stations, enable/disable. |
| `/fares` | Fares | Trip/segment/route pricing with auto-close. |
| `/routes` | Routes | Origin → intermediate stops → destination. |
| `/trains` | Trains + coaches + seats | Define rolling stock. |
| `/geography` | Cities + stations | Map-based station placement. |
| `/seeding` | **Seeding (SuperAdmin-only)** | Import demo data, reset, bulk operations. |

### User stories

- As a SuperAdmin, I want to seed an empty database with demo cities, stations,
  routes and trips, so I can spin up a sandbox in one click.
- As a SuperAdmin, I want to promote a regular user to Admin, so trusted
  colleagues can manage day-to-day operations without my involvement.
- As a SuperAdmin, I want to access every page so I can troubleshoot any
  feature without role-switching.

### 📸 Screenshot manifest — `screens/superadmin/`

| # | Filename | What it shows |
| --- | --- | --- |
| 01 | `01-login.png` | Admin login screen. |
| 02 | `02-dashboard-stats.png` | Dashboard with stats: trains, routes, stations, today's trip count. |
| 03 | `03-geography-cities.png` | Geography page → cities list with search + "+ Add city" button. |
| 04 | `04-add-city-modal.png` | Add-city modal: search box → name AR + EN + lat/lng + map preview. |
| 05 | `05-geography-stations.png` | Geography → stations segment with list + "+ Add station" button. |
| 06 | `06-add-station-modal.png` | Add-station modal: map picker, city select, station name fields. |
| 07 | `07-geography-map.png` | Map segment showing all covered cities + stations as pins. |
| 08 | `08-routes-list.png` | Routes page with filter + search + "+ Add route" button. |
| 09 | `09-edit-route-names-modal.png` | Edit route names modal (AR / EN). |
| 10 | `10-route-add-stop-modal.png` | Add intermediate stop modal for a route. |
| 11 | `11-fares-list.png` | Fares page list with "+ Add fare" button. |
| 12a | `12a-add-fare-route.png` | Add fare — **Route scope**: class + price + discount. |
| 12b | `12b-add-fare-segment.png` | Add fare — **Segment scope**: origin/destination + class + price. |
| 12c | `12c-add-fare-trip.png` | Add fare — **Trip scope**: pinned trip + class + price. |
| 13 | `13-edit-fare-modal.png` | Edit fare modal: price / start date / discount. |
| 14 | `14-trains-list.png` | Trains page with "+ Add train" button. |
| 15 | `15-add-train-modal.png` | Add-train modal: number, name AR + EN. |
| 16 | `16-train-coaches-modal.png` | Train coaches management modal (list of coaches). |
| 17 | `17-add-coach-modal.png` | Add-coach modal: number, capacity, class. |
| 18 | `18-trips-list.png` | Trips list page with "+ Add trip" button. |
| 19 | `19-add-trip-modal.png` | Add-trip modal: train + route + departure + arrival dates. |
| 20 | `20-counter-customer-step.png` | Sell-tickets step 1: search existing customer or pick walk-in. |
| 21 | `21-counter-trip-segment.png` | Sell-tickets step 2: trip + boarding + alighting stations. |
| 22 | `22-counter-seats-passengers.png` | Sell-tickets step 3: seat grid + passenger info forms + confirm. |
| 23 | `23-boarding-manifest.png` | Boarding portal: trip's ticket list with Board / NoShow actions. |
| 24 | `24-users-list.png` | Users page: list with roles chips + station chips + "+ Add user". |
| 25 | `25-add-user-modal.png` | Add-user modal: name, email, phone, password, role(s). |
| 26 | `26-edit-user-info-modal.png` | Edit user basics modal: name, email, phone. |
| 27 | `27-edit-user-roles-modal.png` | Assign-roles modal (role checkbox list). |
| 28 | `28-edit-user-stations-modal.png` | Assign-stations modal (station checkbox list). |
| 29 | `29-user-active-confirm.png` | Enable/disable user confirmation dialog. |

---

## 2. Admin

### Persona
Operations manager. Owns the catalog (trips, routes, fares, fleet) and the
staff roster. Does not seed data, but otherwise has full reach.

### Pages reachable

Same as SuperAdmin **except** `/seeding`.

### User stories

#### Users + staff management (`/users`)

- As an Admin, I want to add a new staff member with username + email +
  password, so they can log in to the admin app.
- As an Admin, I want to assign a user the `StaffCounter` role and pin them to
  Khartoum + Atbara, so they can only sell tickets at those two counters.
- As an Admin, I want to add the `StaffBoarding` role to the same user, so
  small-station agents who do both jobs share one account.
- As an Admin, I want to disable a user's account, so a former employee can no
  longer log in even if they kept their password.
- As an Admin, I want to search users by name / phone / email, so I can find a
  customer record quickly when they call support.

#### Trips, routes, fares, fleet

- As an Admin, I want to create a new daily trip on the Khartoum→Wad Madani
  route with a specific train, so it shows up in the customer search.
- As an Admin, I want to set a trip-specific fare for the upcoming Eid surge,
  so customers see the premium price for that trip only.
- As an Admin, I want to cancel a trip with a reason note, so all affected
  bookings flip to Cancelled, refunds get queued, and customers see a
  notification.
- As an Admin, I want to define a route with intermediate stops in order
  (origin → hasahisa → destination), so trips on this route can sell tickets
  per segment.

#### Refunds (`/refunds`)

- As an Admin, I want to see pending refunds and approve them, so the
  cancellation flow completes.
- As an Admin, I want to reject a fraudulent refund with a reason, so the
  customer sees why and the booking history reflects it.

#### Cross-cutting

- As an Admin, I want a global view of trips for any date, so I can plan
  capacity ahead of time without station scope getting in my way.
- As an Admin, I want to override boarding actions at any station, so I can
  help when a station agent is unavailable.

### 📸 Screenshot manifest — Admin

All Admin screens are **the same shots as SuperAdmin #02 → #29** (Admin sees
everything except `/seeding`). Reuse those captures. No fresh shots needed
under `screens/admin/`.

---

## 3. StaffCounter

### Persona
The ticket-window cashier at a station. Sells tickets to customers in front of
the desk; never boards passengers; cannot edit catalog data.

### Pages reachable

| URL | Page | Visible? | What they can do |
| --- | --- | --- | --- |
| `/dashboard` | Dashboard | ✓ | Today's summary at their stations. |
| `/counter` | Counter sale | ✓ | Sell tickets — the primary screen. |
| `/trips` | Trips | ✓ (read-only) | View only trips touching their station(s). |
| `/bookings` | Bookings | ✓ (read-only) | View bookings; cannot cancel. |
| `/boarding`, `/users`, `/refunds`, `/fares`, `/routes`, `/trains`, `/geography`, `/seeding` | — | ✗ | Hidden from sidebar; route-guarded. |

### User stories

#### Step 1 — pick the customer

- As a StaffCounter, I want to search a customer by phone or email, so I can
  attach the booking to their account and they see it on their app dashboard.
- As a StaffCounter, I want a "Walk-in (no account)" option, so I can sell a
  ticket to a customer who never registered.
- As a StaffCounter, I want the first passenger form to auto-fill with the
  picked customer's name / phone / ID, so I don't retype data they already
  gave us.

#### Step 2 — pick the trip + segment

- As a StaffCounter, I want to see only **upcoming** trips whose train hasn't
  departed my station yet, so I don't accidentally sell a ticket for a train
  that already left.
- As a StaffCounter, I want my **boarding station** locked to the station(s)
  I'm assigned to, so I can't sell a Port Sudan-boarding ticket while sitting
  at the Khartoum counter.
- As a StaffCounter, I want a clear list of destination stops, so the customer
  can pick where they're going.

#### Step 3 — pick seats + passenger info

- As a StaffCounter, I want to see the same visual seat grid the customer app
  shows, so I can point at the screen and let the customer choose.
- As a StaffCounter, I want occupied seats marked red and selectable seats
  marked white, so I never offer a taken seat.
- As a StaffCounter, I want each picked seat to show its ordinal number, so
  the customer can verify "Yes, I want seats 1 and 2".
- As a StaffCounter, I want a class filter (First / Second / Third), so I can
  hide expensive options when the customer wants the cheapest.
- As a StaffCounter, I want the per-passenger form to require Arabic name,
  English name, ID, birth date, gender, nationality and phone, so the printed
  ticket is complete and valid for inspection.
- As a StaffCounter, I want validation errors highlighted in red before I
  submit, so I don't get a server rejection after the customer paid.

#### Submit

- As a StaffCounter, I want payment locked to Cash, so I can't accidentally
  pick the wrong method when collecting bills.
- As a StaffCounter, I want a confirmation toast + printable ticket, so I can
  hand the customer their seat numbers.

### 📸 Screenshot manifest — `screens/staffcounter/`

The sell-tickets flow is the same component as SuperAdmin's. **Capture once
under SuperAdmin and reuse the references here.** Only the read-only pages
need fresh shots.

| # | Filename | Source / what it shows |
| --- | --- | --- |
| 01 | *(reuse)* `superadmin/20-counter-customer-step.png` | Sell-tickets — pick customer. **Shared with SuperAdmin #20.** |
| 02 | *(reuse)* `superadmin/21-counter-trip-segment.png` | Sell-tickets — trip + stations. **Shared with SuperAdmin #21.** |
| 03 | *(reuse)* `superadmin/22-counter-seats-passengers.png` | Sell-tickets — seats + passenger info. **Shared with SuperAdmin #22.** |
| 04 | `04-staffcounter-trips-readonly.png` | Trips page narrowed to the agent's station (no create / edit / cancel actions visible). |
| 05 | `05-staffcounter-bookings-readonly.png` | Bookings page narrowed to the agent's station (no Cancel button per row). |

> If the StaffCounter sell-tickets UI ends up visually different from the
> SuperAdmin one (e.g. boarding-station dropdown is constrained), capture a
> separate `staffcounter/02-trip-segment-constrained.png` and reference the
> difference.

---

## 4. StaffBoarding

### Persona
The platform / gate agent. Verifies passengers boarding the train, marks
no-shows, and flips trip status to Departed / Arrived. Never sells tickets.

### Pages reachable

| URL | Page | Visible? | What they can do |
| --- | --- | --- | --- |
| `/dashboard` | Dashboard | ✓ | Today's manifests at their station(s). |
| `/boarding` | Boarding portal | ✓ | The primary screen. |
| `/trips` | Trips | ✓ (read-only) | Eye icon → jump to manifest. |
| `/bookings` | Bookings | ✓ (read-only) | View only; cannot cancel. |
| `/counter`, `/users`, `/refunds`, `/fares`, `/routes`, `/trains`, `/geography`, `/seeding` | — | ✗ | Hidden + guarded. |

### User stories

#### Trip picker + manifest

- As a StaffBoarding, I want to see only trips that touch my station and
  haven't departed yet, so my picker isn't cluttered with finished trips.
- As a StaffBoarding, I want the manifest to default to passengers boarding
  **at my station**, so I don't see passengers boarding at upstream stations
  who already got on the train.
- As a StaffBoarding, I want per-row passenger info (Arabic name, ID, seat,
  coach, boarding → alighting), so I can verify identity at a glance.
- As a StaffBoarding, I want the status counts in the header (boarded /
  issued / no-show / cancelled), so I know progress without scrolling.

#### Boarding actions

- As a StaffBoarding, I want a "Scan QR" button that lets me paste a QR
  payload or type a ticket number, so I can verify tickets in any state.
- As a StaffBoarding, I want a per-row "Board" button, so I can board a
  passenger whose phone died or whose QR is unreadable.
- As a StaffBoarding, I want a "Mark no-show" button on each Issued row, so I
  can close out the trip's manifest after departure.
- As a StaffBoarding, I want the button labels and badges to flip color when
  a ticket goes Issued → Boarded, so I don't board the same person twice.

#### Trip transitions

- As a StaffBoarding, I want "Trip departed" and "Trip arrived" buttons in
  the manifest header, so I can flip the trip status from the same screen.
- As a StaffBoarding, I want trip-cancel to NOT be available to me — only
  Admin/SuperAdmin should hold that destructive power.

### 📸 Screenshot manifest — `screens/staffboarding/`

The boarding page is the same component as SuperAdmin's. **Capture once under
SuperAdmin and reuse the reference here.**

| # | Filename | Source / what it shows |
| --- | --- | --- |
| 01 | *(reuse)* `superadmin/23-boarding-manifest.png` | Trip picker + manifest with Board / NoShow actions. **Shared with SuperAdmin #23.** |
| 02 | `02-staffboarding-trips.png` | Trips page narrowed to the agent's station (read-only). |
| 03 | `03-staffboarding-bookings.png` | Bookings page narrowed to the agent's station (read-only). |
| 04 | `04-scan-modal.png` | *(optional)* QR-scan paste modal. |

---

## 5. Customer

### Persona
A regular passenger booking tickets from a phone or laptop. Arabic-only RTL
interface. Self-registered.

### Pages reachable

| URL | Page | What it does |
| --- | --- | --- |
| `/` | Homepage | Marketing landing + search shortcut. |
| `/login` `/register` | Auth | Self-registration with email confirmation. |
| `/search` | Search results | Trips matching origin / destination / date / passenger count, with starting-from prices. |
| `/book` | Booking flow | 4 steps: passenger info → seat select → payment → confirmation. |
| `/dashboard` | My trips | Upcoming + past tabs; e-ticket modal with QR; cancellation flow. |
| Header bell | Notifications drawer | Booking / trip cancellation alerts. |

### User stories

#### Discover + search

- As a customer, I want to search trips by origin, destination, and date, so
  I see only the trips that get me where I'm going.
- As a customer, I want to see prices starting from the cheapest available
  class on each trip, so I can compare quickly.
- As a customer, I want a gold "trip price" chip when this specific trip has
  a special fare, so I understand why the price differs from the route's
  default.

#### Book

- As a customer, I want to enter passenger info with red-flagged validation
  errors, so I know what to fix before paying.
- As a customer, I want to pick multiple seats at once for the family, so we
  sit together.
- As a customer, I want occupied seats marked red and my picks marked gold
  with ordinal numbers, so I never accidentally book a taken seat.
- As a customer, I want a 422 "seat taken" error to bounce me back to the
  seat grid with refreshed availability, so I can pick again without losing
  passenger data I already entered.
- As a customer, I want to pay with Card / Mobile Wallet / Bank Transfer /
  Cash, so I can use my preferred method.
- As a customer, I want a confirmation page with the booking reference and
  printable QR per ticket, so I have proof to show at the gate.

#### Manage trips

- As a customer, I want a dashboard with Upcoming / Past tabs, so I can find
  trips by their state.
- As a customer, I want the e-ticket modal to show **each ticket's status**
  (Issued / Boarded / NoShow / Cancelled), so I know who got on and who didn't.
- As a customer, I want to cancel an upcoming booking with a reason, so I
  trigger the refund flow before the train leaves.
- As a customer, I want to see a small bell in the header with the unread
  count, so I notice when a trip got cancelled.
- As a customer, I want a notifications drawer that auto-marks items read
  when opened, so I'm not nagged by old alerts.

### 📸 Screenshot manifest — `screens/customer/`

| # | Filename | What it shows |
| --- | --- | --- |
| 01 | `01-register.png` | Register page — first name, last name, email, password, confirm. |
| 02 | `02-confirm-email-otp.png` | OTP code entry after register. |
| 03 | `03-login.png` | Login screen after email confirmed. |
| 04 | `04-home-search.png` | Homepage with origin / destination / date / passenger count selectors. |
| 05 | `05-trips-results.png` | Available trips list with starting-from prices. |
| 06 | `06-booking-passenger-info.png` | Passenger personal info form (accordion). |
| 07 | `07-booking-seat-select.png` | Visual seat grid grouped by coach class. |
| 08 | `08-booking-payment.png` | Payment method selection + confirm. |
| 09 | `09-booking-confirmation-qr.png` | Confirmation screen with QR code + "go to my trips" link. |
| 10 | `10-my-trips.png` | Dashboard: upcoming + past tabs with booking cards. |
| 11 | `11-cancel-confirm-dialog.png` | Cancel-trip dialog (destructive confirm). |

---

## Summary — fresh screenshots to capture

| Role | Fresh captures |
| --- | :-: |
| Customer | 11 |
| SuperAdmin | 31 (12a + 12b + 12c counted) |
| Admin | 0 (reuses SuperAdmin #02–#29) |
| StaffCounter | 2 |
| StaffBoarding | 2 (or 3 if you add the scan modal) |
| **Total** | **~46 screens** |

## Folder structure to upload

## Page-access matrix (reference)

| Page | SuperAdmin | Admin | StaffCounter | StaffBoarding | Customer |
| --- | :-: | :-: | :-: | :-: | :-: |
| `/dashboard` | ✓ | ✓ | ✓ | ✓ | ✓ (customer app) |
| `/counter` | ✓ | ✓ | ✓ | ✗ | ✗ |
| `/boarding` | ✓ | ✓ | ✗ | ✓ | ✗ |
| `/trips` (admin) | ✓ | ✓ | ✓ read-only | ✓ read-only | ✗ |
| `/bookings` (admin) | ✓ | ✓ | ✓ read-only | ✓ read-only | ✗ |
| `/refunds` | ✓ | ✓ | ✗ | ✗ | ✗ |
| `/users` | ✓ | ✓ | ✗ | ✗ | ✗ |
| `/fares` | ✓ | ✓ | ✗ | ✗ | ✗ |
| `/routes` | ✓ | ✓ | ✗ | ✗ | ✗ |
| `/trains` | ✓ | ✓ | ✗ | ✗ | ✗ |
| `/geography` | ✓ | ✓ | ✗ | ✗ | ✗ |
| `/seeding` | ✓ | ✗ | ✗ | ✗ | ✗ |
| Customer `/search`, `/book`, `/dashboard` | — | — | — | — | ✓ |

---

## Suggested slide layout

1. **Title slide** — Sudan Trains: User Stories by Role.
2. **Roles overview** — single slide with the 5-role page-access matrix above.
3. **For each role (5 sections)**:
   - **Cover slide** — role name, persona, one-line summary.
   - **Pages slide(s)** — one screenshot per slide on the right, the 4-6
     relevant stories on the left. Use the screenshot manifest filename.
4. **Cross-role flow** — one slide showing a complete journey: Customer books
   → StaffBoarding scans → ticket flips to Boarded.
5. **Closing slide** — out-of-scope list (HMAC QR, real payment-gateway
   refund, SignalR live updates, native camera bridge).

## Prompt for claude.ai

When the screenshots folder is ready, upload `docs/user-stories.md` plus the
`docs/screens/` folder to a new claude.ai conversation and paste:

> Build a PowerPoint deck from this document. Use the role sections as the
> narrative and the screenshot manifests to know which image file to drop on
> each slide. Follow the slide layout described at the bottom of the file.
> When a manifest says "reuse" or "shared with", point both slides to the
> same image file. Use a clean RTL-aware layout — labels and bullets in
> Arabic where the screen shows Arabic content.