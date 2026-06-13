import { useEffect, useMemo, useState } from 'react';
import { tripsApi, usersApi, bookingsApi, stationsApi, routesApi } from '../services/api';
import {
  Trip,
  Route,
  CustomerSummary,
  CounterBookingPayload,
  CounterSeatInput,
  SegmentSeatsDto,
  AvailableSeatDto,
  CoachSeatsDto,
  coachClassNameToId,
} from '../types/infrastructure';
import type { Station } from '../types/geography';
import { AR } from '../i18n/ar';
import { showError, showSuccess, extractErrorMessage } from '../utils/alerts';
import { useMe } from '../contexts/MeContext';

// Stop on a trip's route in order (origin → intermediates by stopOrder →
// destination). Used to populate the boarding/alighting selects so the
// counter agent only ever picks a station that's actually on the route.
interface RouteStop {
  stationId: number;
  stationName: string;
  stopOrder: number;
}

// Human-readable departure time for option labels.
function fmtDeparture(iso: string): string {
  const d = new Date(iso);
  if (isNaN(d.getTime())) return iso;
  return d.toLocaleString('ar-EG', { dateStyle: 'medium', timeStyle: 'short' });
}

// Counter sale flow: pick customer → pick trip → enter passenger(s) → confirm.
// Seat selection is simplified to a single seatId per passenger entry
// (entered manually) for MVP — the customer-app seat grid lives in the
// customer app; counter staff can call out seat numbers from the system.

const CounterBookingPage = () => {
  const { me, isAdmin } = useMe();
  const [step, setStep] = useState<1 | 2 | 3>(1);

  // Stations the agent is assigned to — used to constrain the boarding-station
  // input when the user is a non-admin StaffCounter. Admin keeps free entry.
  const assignedStationIds = me?.assignedStationIds ?? [];
  const restrictBoarding = !isAdmin && assignedStationIds.length > 0;
  const [assignedStations, setAssignedStations] = useState<Station[]>([]);
  useEffect(() => {
    if (!restrictBoarding) { setAssignedStations([]); return; }
    stationsApi.getAll()
      .then((all) => setAssignedStations(all.filter((s) => assignedStationIds.includes(s.id))))
      .catch(() => setAssignedStations([]));
  }, [restrictBoarding, assignedStationIds.join(',')]);

  const boardingOptions = useMemo(() => assignedStations.map((s) => ({
    id: s.id,
    label: `${s.nameAr} (${s.code})`,
  })), [assignedStations]);

  // Step 1 — customer.
  const [walkIn, setWalkIn] = useState(false);
  const [search, setSearch] = useState('');
  const [matches, setMatches] = useState<CustomerSummary[]>([]);
  const [picked, setPicked] = useState<CustomerSummary | null>(null);

  // Step 2 — trip + seats.
  const [trips, setTrips] = useState<Trip[]>([]);
  const [tripId, setTripId] = useState<number | ''>('');
  const [boardingStationId, setBoardingStationId] = useState<number | ''>('');
  const [alightingStationId, setAlightingStationId] = useState<number | ''>('');

  // Selected trip → route → ordered stops. Populated whenever tripId changes
  // so the boarding + alighting selects list real stations on the route.
  const selectedTrip = useMemo(() => trips.find((t) => t.id === tripId) ?? null, [trips, tripId]);
  const [routeStops, setRouteStops] = useState<RouteStop[]>([]);
  useEffect(() => {
    if (!selectedTrip) { setRouteStops([]); return; }
    routesApi.getById(selectedTrip.routeId)
      .then((r: Route) => {
        const stops: RouteStop[] = [
          { stationId: r.origin.id, stationName: r.origin.nameAr || r.origin.nameEn || r.origin.code, stopOrder: 0 },
          ...r.intermediateStops
            .slice()
            .sort((a, b) => a.stopOrder - b.stopOrder)
            .map((s) => ({ stationId: s.stationId, stationName: s.stationName, stopOrder: s.stopOrder })),
          { stationId: r.destination.id, stationName: r.destination.nameAr || r.destination.nameEn || r.destination.code, stopOrder: Number.MAX_SAFE_INTEGER },
        ];
        setRouteStops(stops);
      })
      .catch(() => setRouteStops([]));
    // Reset selections — they belong to the previous trip.
    setBoardingStationId('');
    setAlightingStationId('');
  }, [selectedTrip]);

  // Boarding options: all route stops, narrowed to the agent's assigned set
  // when the user is a non-admin StaffCounter.
  const boardingStopOptions = useMemo(() => {
    if (restrictBoarding) {
      return routeStops.filter((s) => assignedStationIds.includes(s.stationId));
    }
    return routeStops;
  }, [routeStops, restrictBoarding, assignedStationIds.join(',')]);

  // Alighting options: stops strictly downstream of the chosen boarding stop.
  const alightingStopOptions = useMemo(() => {
    if (boardingStationId === '' || routeStops.length === 0) return [];
    const boarding = routeStops.find((s) => s.stationId === boardingStationId);
    if (!boarding) return [];
    return routeStops.filter((s) => s.stopOrder > boarding.stopOrder);
  }, [routeStops, boardingStationId]);

  // Step 3 — passengers. Payment is always Cash for counter sales.
  // Each entry holds the per-passenger info form ONLY; the seat is paired
  // by ordinal position from `selectedSeats` (matches the customer flow).
  const makeEmptyPassenger = (): CounterSeatInput => ({
    seatId: 0,
    coachClass: 2, // Second — overwritten at submit from the chosen seat's coach.
    passenger: {
      fullNameEn: '',
      fullNameAr: '',
      idNumber: '',
      phone: '',
      email: '',
      gender: 'male',
      nationality: 'Sudan',
      birthDate: '',
    },
  });
  const [passengers, setPassengers] = useState<CounterSeatInput[]>([makeEmptyPassenger()]);
  const [expandedPassengerIdx, setExpandedPassengerIdx] = useState(0);
  // Per-passenger field-touched tracking. Errors only show on touched fields
  // until the user tries to submit, after which every field's error is shown.
  const [touched, setTouched] = useState<Record<number, Record<string, boolean>>>({});
  const [showAllErrors, setShowAllErrors] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  // Per-segment seat availability for the chosen boarding→alighting leg.
  // Fetched lazily once both stops are picked; refreshed when either changes.
  const [segmentSeats, setSegmentSeats] = useState<SegmentSeatsDto | null>(null);
  const [selectedSeats, setSelectedSeats] = useState<AvailableSeatDto[]>([]);
  // Class filter chip — '' means show all coaches.
  const [chosenClass, setChosenClass] = useState<string>('');

  useEffect(() => {
    if (tripId === '' || boardingStationId === '' || alightingStationId === '') {
      setSegmentSeats(null);
      setSelectedSeats([]);
      return;
    }
    tripsApi.getSegmentSeats(tripId, boardingStationId, alightingStationId)
      .then((s) => {
        setSegmentSeats(s);
        // Drop any previously-picked seat that is no longer available.
        setSelectedSeats((prev) => {
          const stillThere = new Map<number, AvailableSeatDto>();
          s.coaches.forEach((c) => c.seats.forEach((seat) => {
            if (seat.isAvailable) stillThere.set(seat.id, seat);
          }));
          return prev.filter((p) => stillThere.has(p.id));
        });
      })
      .catch(() => setSegmentSeats(null));
  }, [tripId, boardingStationId, alightingStationId]);

  // Lookup: seatId → { coach, seat } — used for ordinal pairing + payload submit.
  const seatLookup = useMemo(() => {
    const m = new Map<number, { coachClass: string; seatNumber: string; coachNumber: string }>();
    segmentSeats?.coaches.forEach((c) => {
      c.seats.forEach((s) => {
        m.set(s.id, { coachClass: c.class, seatNumber: s.seatNumber, coachNumber: c.coachNumber });
      });
    });
    return m;
  }, [segmentSeats]);

  // Coaches filtered by the class chip (or all coaches when '').
  const visibleCoaches = useMemo<CoachSeatsDto[]>(() => {
    if (!segmentSeats) return [];
    if (!chosenClass) return segmentSeats.coaches;
    return segmentSeats.coaches.filter((c) => c.class === chosenClass);
  }, [segmentSeats, chosenClass]);

  // Customer-style toggle: click an available seat → it goes to the next
  // empty passenger slot. Click a selected seat → deselect (ordinals renumber).
  const toggleSeat = (seat: AvailableSeatDto) => {
    if (!seat.isAvailable) return;
    setSelectedSeats((prev) => {
      const i = prev.findIndex((p) => p.id === seat.id);
      if (i >= 0) return prev.filter((_, idx) => idx !== i);
      if (prev.length >= passengers.length) return prev;
      return [...prev, seat];
    });
  };

  useEffect(() => {
    if (step === 2) {
      const today = new Date().toISOString().slice(0, 10);
      tripsApi.getAll({ date: today, upcomingOnly: true })
        .then(setTrips)
        .catch((err) => showError(AR.common.errorTitle, extractErrorMessage(err)));
    }
  }, [step]);

  useEffect(() => {
    if (search.trim().length < 2 || walkIn) { setMatches([]); return; }
    const id = setTimeout(() => {
      usersApi.lookup(search.trim()).then(setMatches).catch(() => setMatches([]));
    }, 250);
    return () => clearTimeout(id);
  }, [search, walkIn]);

  // Pre-fill first passenger when a registered customer is selected. The
  // backend lookup gives a single "fullName" string — route it into the
  // Arabic OR English bucket based on whether it contains Arabic letters.
  useEffect(() => {
    if (!picked) return;
    const name = picked.fullName ?? '';
    const isArabic = /[؀-ۿ]/.test(name);
    setPassengers((prev) => {
      const next = [...prev];
      next[0] = {
        ...next[0],
        passenger: {
          ...next[0].passenger,
          fullNameAr: isArabic ? name : next[0].passenger.fullNameAr,
          fullNameEn: !isArabic ? name : next[0].passenger.fullNameEn,
          phone: picked.phoneNumber || next[0].passenger.phone,
          email: picked.email || next[0].passenger.email,
          idNumber: picked.idNumber || next[0].passenger.idNumber,
        },
      };
      return next;
    });
  }, [picked]);

  // ----- Validation (mirror of the customer-app validatePassenger) -----
  type PassengerErrors = Partial<Record<keyof CounterSeatInput['passenger'], string>>;

  const validatePassenger = (p: CounterSeatInput['passenger']): PassengerErrors => {
    const errs: PassengerErrors = {};
    const arName = (p.fullNameAr ?? '').trim();
    if (!arName) errs.fullNameAr = AR.counter.validationRequired;
    else if (!/^[؀-ۿ\s]+$/.test(arName)) errs.fullNameAr = AR.counter.validationArabic;

    const enName = (p.fullNameEn ?? '').trim();
    if (!enName) errs.fullNameEn = AR.counter.validationRequired;
    else if (!/^[A-Za-z\s]+$/.test(enName)) errs.fullNameEn = AR.counter.validationEnglish;

    const idn = (p.idNumber ?? '').trim();
    if (!idn) errs.idNumber = AR.counter.validationRequired;
    else if (!/^[A-Za-z0-9]{5,30}$/.test(idn)) errs.idNumber = AR.counter.validationIdFormat;

    if (!p.birthDate) {
      errs.birthDate = AR.counter.validationRequired;
    } else {
      const bd = new Date(p.birthDate);
      if (isNaN(bd.getTime())) errs.birthDate = AR.counter.validationDateInvalid;
      else {
        const now = new Date();
        if (bd > now) errs.birthDate = AR.counter.validationBirthFuture;
        else if (now.getFullYear() - bd.getFullYear() > 120)
          errs.birthDate = AR.counter.validationBirthTooOld;
      }
    }

    const ph = (p.phone ?? '').trim();
    if (!ph) errs.phone = AR.counter.validationRequired;
    else if (!/^\+?[0-9]{8,15}$/.test(ph.replace(/\s/g, ''))) errs.phone = AR.counter.validationPhoneFormat;

    const em = (p.email ?? '').trim();
    if (em && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(em)) errs.email = AR.counter.validationEmailFormat;

    return errs;
  };

  const passengerErrors = useMemo(
    () => passengers.map((p) => validatePassenger(p.passenger)),
    [passengers],
  );
  const anyValidationErrors = passengerErrors.some((e) => Object.keys(e).length > 0);

  const markTouched = (i: number, field: keyof CounterSeatInput['passenger']) =>
    setTouched((prev) => ({ ...prev, [i]: { ...(prev[i] ?? {}), [field]: true } }));

  const errorFor = (i: number, field: keyof CounterSeatInput['passenger']): string | undefined => {
    const err = passengerErrors[i]?.[field];
    if (!err) return undefined;
    if (showAllErrors) return err;
    return touched[i]?.[field] ? err : undefined;
  };

  const isPassengerFilled = (p: CounterSeatInput['passenger']) =>
    !!(p.fullNameAr && p.fullNameEn && p.idNumber && p.birthDate && p.phone);

  const passengerHasVisibleErrors = (i: number) =>
    Object.keys(passengerErrors[i] ?? {}).some(
      (f) => showAllErrors || touched[i]?.[f] === true,
    );

  const togglePassengerPanel = (i: number) =>
    setExpandedPassengerIdx((cur) => (cur === i ? -1 : i));

  const addPassenger = () => {
    setPassengers((p) => [...p, makeEmptyPassenger()]);
    setExpandedPassengerIdx(passengers.length); // expand the new row
  };

  // Remove the i-th passenger AND any seat that was paired to a passenger
  // index that no longer exists (last-N drop is the simplest mental model).
  const removePassenger = (i: number) => {
    setPassengers((p) => p.filter((_, idx) => idx !== i));
    setSelectedSeats((prev) => prev.slice(0, Math.max(0, prev.length - 1)));
  };

  const updatePassengerInfo = (i: number, key: keyof CounterSeatInput['passenger'], value: string) =>
    setPassengers((p) => p.map((ps, idx) => (idx === i ? { ...ps, passenger: { ...ps.passenger, [key]: value } } : ps)));

  const submit = async () => {
    if (tripId === '' || boardingStationId === '' || alightingStationId === '') {
      showError(AR.common.errorTitle, 'Trip / segment required.');
      return;
    }
    if (selectedSeats.length !== passengers.length) {
      showError(AR.common.errorTitle, AR.counter.pickSeatFirst);
      return;
    }
    // Block submit if any passenger row has validation errors. Flip the
    // touched-mask so errors become visible on every field at once.
    if (anyValidationErrors) {
      setShowAllErrors(true);
      const firstBad = passengerErrors.findIndex((e) => Object.keys(e).length > 0);
      if (firstBad >= 0) setExpandedPassengerIdx(firstBad);
      showError(AR.common.errorTitle, AR.counter.fixErrors);
      return;
    }

    // Pair the i-th selected seat with the i-th passenger row. coachClass is
    // derived from the seat's coach (via seatLookup) — single source of truth.
    // Convert the string class name from SegmentSeatsDto to the numeric
    // backend enum value (1=First, 2=Second, 3=Third).
    const enriched: CounterSeatInput[] = passengers.map((ps, i) => {
      const seat = selectedSeats[i];
      const meta = seatLookup.get(seat.id);
      return {
        seatId: seat.id,
        coachClass: meta ? coachClassNameToId(meta.coachClass) : ps.coachClass,
        passenger: ps.passenger,
      };
    });

    const payload: CounterBookingPayload = {
      customerUserId: walkIn ? null : picked?.userId ?? null,
      tripId,
      boardingStationId,
      alightingStationId,
      paymentMethod: 0, // Cash — counter sales are cash-only.
      passengers: enriched,
    };
    setSubmitting(true);
    try {
      await bookingsApi.createCounter(payload);
      await showSuccess(AR.counter.bookingCreated);
      // Reset.
      setStep(1); setPicked(null); setWalkIn(false); setSearch(''); setMatches([]);
      setTripId(''); setBoardingStationId(''); setAlightingStationId('');
      setPassengers([makeEmptyPassenger()]);
      setSelectedSeats([]);
      setChosenClass('');
      setTouched({});
      setShowAllErrors(false);
      setExpandedPassengerIdx(0);
    } catch (err) {
      showError(AR.common.errorTitle, extractErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div>
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-900">{AR.counter.title}</h1>
        <p className="text-gray-600">{AR.counter.subtitle}</p>
      </div>

      <ol className="flex gap-2 mb-4 text-sm">
        {([1, 2, 3] as const).map((n) => (
          <li key={n} className={`px-3 py-1.5 rounded-md border ${step === n ? 'bg-admin-primary-50 border-admin-primary-200' : 'bg-white'}`}>
            {n}
          </li>
        ))}
      </ol>

      {step === 1 && (
        <div className="admin-card p-4 space-y-3">
          <div className="flex gap-2">
            <button className={!walkIn ? 'admin-button' : 'admin-button-secondary'} onClick={() => setWalkIn(false)}>
              {AR.counter.pickRegistered}
            </button>
            <button className={walkIn ? 'admin-button' : 'admin-button-secondary'} onClick={() => { setWalkIn(true); setPicked(null); }}>
              {AR.counter.pickWalkIn}
            </button>
          </div>

          {!walkIn && (
            <>
              <input
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                placeholder={AR.counter.lookupPlaceholder}
                className="w-full border rounded-md px-3 py-2 text-sm"
              />
              <ul className="space-y-1 max-h-60 overflow-y-auto">
                {matches.map((c) => (
                  <li key={c.userId}>
                    <button
                      onClick={() => setPicked(c)}
                      className={`w-full text-start border rounded-md px-3 py-2 hover:bg-gray-50 ${picked?.userId === c.userId ? 'bg-admin-primary-50' : ''}`}
                    >
                      <div className="font-semibold">{c.fullName}</div>
                      <div className="text-xs text-gray-500">
                        {[c.userName, c.email, c.phoneNumber].filter(Boolean).join(' • ')}
                      </div>
                    </button>
                  </li>
                ))}
                {search.trim().length >= 2 && matches.length === 0 && (
                  <li className="text-sm text-gray-500 px-1">{AR.counter.noMatches}</li>
                )}
              </ul>
              {picked && (
                <div className="text-sm text-green-700">{AR.counter.selected}: <strong>{picked.fullName}</strong></div>
              )}
            </>
          )}

          <div className="flex justify-end">
            <button className="admin-button" disabled={!walkIn && !picked} onClick={() => setStep(2)}>
              {AR.common.confirm}
            </button>
          </div>
        </div>
      )}

      {step === 2 && (
        <div className="admin-card p-4 space-y-3">
          <label className="block">
            <span className="text-sm text-gray-700">{AR.boarding.pickTrip}</span>
            <select
              value={tripId}
              onChange={(e) => setTripId(e.target.value ? Number(e.target.value) : '')}
              className="w-full max-w-md border rounded-md px-3 py-1.5 text-sm mt-1"
            >
              <option value="">{AR.common.selectPlaceholder}</option>
              {trips.map((t) => (
                <option key={t.id} value={t.id}>#{t.id} — {t.routeName} — {fmtDeparture(t.departureTime)}</option>
              ))}
            </select>
          </label>
          <div className="grid grid-cols-2 gap-3">
            <label className="block">
              <span className="text-sm text-gray-700">{AR.boarding.from}</span>
              <select
                value={boardingStationId}
                onChange={(e) => setBoardingStationId(e.target.value ? Number(e.target.value) : '')}
                disabled={!selectedTrip || boardingStopOptions.length === 0}
                className="w-full border rounded-md px-3 py-1.5 text-sm mt-1 disabled:bg-gray-100"
              >
                <option value="">{AR.common.selectPlaceholder}</option>
                {boardingStopOptions.map((s) => (
                  <option key={s.stationId} value={s.stationId}>{s.stationName}</option>
                ))}
              </select>
              {selectedTrip && boardingStopOptions.length === 0 && restrictBoarding && (
                <span className="block text-xs text-red-600 mt-1">
                  {AR.common.none}
                </span>
              )}
            </label>
            <label className="block">
              <span className="text-sm text-gray-700">{AR.boarding.to}</span>
              <select
                value={alightingStationId}
                onChange={(e) => setAlightingStationId(e.target.value ? Number(e.target.value) : '')}
                disabled={boardingStationId === '' || alightingStopOptions.length === 0}
                className="w-full border rounded-md px-3 py-1.5 text-sm mt-1 disabled:bg-gray-100"
              >
                <option value="">{AR.common.selectPlaceholder}</option>
                {alightingStopOptions.map((s) => (
                  <option key={s.stationId} value={s.stationId}>{s.stationName}</option>
                ))}
              </select>
            </label>
          </div>
          <div className="flex justify-between">
            <button className="admin-button-secondary" onClick={() => setStep(1)}>{AR.common.cancel}</button>
            <button className="admin-button" onClick={() => setStep(3)}
              disabled={tripId === '' || boardingStationId === '' || alightingStationId === ''}>
              {AR.common.confirm}
            </button>
          </div>
        </div>
      )}

      {step === 3 && (
        <div className="admin-card p-4 space-y-3">
          {!segmentSeats ? (
            <p className="text-sm text-gray-500">{AR.common.loading}</p>
          ) : (
            <>
              {/* Class filter chips — mirror the customer-app booking flow. */}
              <div className="flex flex-wrap gap-2">
                <button
                  type="button"
                  onClick={() => setChosenClass('')}
                  className={`px-3 py-1 rounded-full text-xs border ${chosenClass === '' ? 'bg-sudan-green-700 text-white border-sudan-green-700' : 'bg-white text-gray-700 border-gray-300'}`}
                >
                  {AR.counter.allClasses}
                </button>
                {Array.from(new Set(segmentSeats.coaches.map((c) => c.class))).map((cls) => (
                  <button
                    key={cls}
                    type="button"
                    onClick={() => setChosenClass(cls)}
                    className={`px-3 py-1 rounded-full text-xs border ${chosenClass === cls ? 'bg-sudan-green-700 text-white border-sudan-green-700' : 'bg-white text-gray-700 border-gray-300'}`}
                  >
                    {cls === 'First' ? AR.counter.firstClass : cls === 'Second' ? AR.counter.secondClass : cls === 'Third' ? AR.counter.thirdClass : cls}
                  </button>
                ))}
              </div>

              {/* Pick counter so the agent knows the cap. */}
              <p className="text-xs text-gray-500">
                {AR.counter.pickedOfTotal.replace('{n}', String(selectedSeats.length)).replace('{m}', String(passengers.length))}
              </p>

              {/* Per-coach seat grid — copied from customer BookingPage. */}
              <div className="space-y-4">
                {visibleCoaches.map((c) => (
                  <div key={c.id} className="bg-gray-50 rounded-lg p-4 sm:p-6">
                    <div className="text-center text-xs text-gray-500 mb-3">
                      {AR.counter.coachLabel} {c.coachNumber} · {c.class === 'First' ? AR.counter.firstClass : c.class === 'Second' ? AR.counter.secondClass : c.class === 'Third' ? AR.counter.thirdClass : c.class}
                      {' · '}
                      {c.seats.filter((s) => s.isAvailable).length} {AR.counter.seatsAvailable}
                    </div>
                    <div className="grid grid-cols-4 gap-1 sm:gap-2 max-w-xs sm:max-w-md mx-auto">
                      {c.seats.map((seat) => {
                        const selectedIdx = selectedSeats.findIndex((p) => p.id === seat.id);
                        const isSelected = selectedIdx >= 0;
                        const atCap = !isSelected && selectedSeats.length >= passengers.length;
                        const klass = isSelected
                          ? 'bg-sudan-gold-500 border-sudan-gold-500 text-sudan-green-900'
                          : !seat.isAvailable
                          ? 'bg-red-200 border-red-300 cursor-not-allowed text-red-700'
                          : atCap
                          ? 'bg-white border-gray-200 text-gray-300 cursor-not-allowed'
                          : 'bg-white border-gray-300 hover:bg-sudan-green-50 hover:border-sudan-green-300';
                        return (
                          <button
                            key={seat.id}
                            type="button"
                            disabled={!seat.isAvailable || atCap}
                            onClick={() => toggleSeat(seat)}
                            className={`relative w-8 h-8 sm:w-10 sm:h-10 rounded border-2 transition-colors text-[10px] sm:text-xs font-medium ${klass}`}
                            title={seat.seatNumber}
                          >
                            {seat.seatNumber}
                            {isSelected && (
                              <span className="absolute -top-1.5 -left-1.5 rtl:left-auto rtl:-right-1.5 w-4 h-4 rounded-full bg-sudan-green-700 text-white text-[9px] flex items-center justify-center">
                                {selectedIdx + 1}
                              </span>
                            )}
                            {seat.isWindow && (
                              <span className="absolute -top-1 -right-1 rtl:right-auto rtl:-left-1 w-1.5 h-1.5 rounded-full bg-sudan-green-500" />
                            )}
                          </button>
                        );
                      })}
                    </div>
                  </div>
                ))}
              </div>

              {/* Legend. */}
              <div className="flex flex-wrap justify-center gap-4 text-xs text-gray-600">
                <span className="flex items-center gap-2"><span className="w-4 h-4 rounded border-2 border-gray-300 bg-white" />{AR.counter.available}</span>
                <span className="flex items-center gap-2"><span className="w-4 h-4 rounded border-2 border-sudan-gold-500 bg-sudan-gold-500" />{AR.counter.selectedSeat}</span>
                <span className="flex items-center gap-2"><span className="w-4 h-4 rounded border-2 border-red-300 bg-red-200" />{AR.counter.occupied}</span>
              </div>

              {/* Passenger-info accordion. Mirrors the customer-app step-1 form. */}
              <div className="mt-2">
                <h2 className="text-lg font-bold text-gray-900">{AR.counter.passengerInfoTitle}</h2>
                <p className="text-xs text-gray-600 mb-3">
                  {AR.counter.passengerCountNote.replace('{n}', String(passengers.length))}
                </p>
                <div className="space-y-2">
                  {passengers.map((ps, i) => {
                    const isOpen = expandedPassengerIdx === i;
                    const visibleErrors = passengerHasVisibleErrors(i);
                    const filled = isPassengerFilled(ps.passenger);
                    const paired = selectedSeats[i];
                    const meta = paired ? seatLookup.get(paired.id) : null;
                    const namePreview = ps.passenger.fullNameAr || ps.passenger.fullNameEn;
                    const errClass = (f: keyof CounterSeatInput['passenger']) =>
                      errorFor(i, f)
                        ? 'border-red-300 focus:border-red-500 focus:ring-red-200'
                        : 'border-gray-300 focus:border-sudan-green-500 focus:ring-sudan-green-200';
                    const baseInput =
                      'w-full px-3 py-2 border rounded-md text-sm focus:outline-none focus:ring-2';
                    return (
                      <div key={i} className={`border rounded-lg overflow-hidden bg-white ${visibleErrors ? 'border-red-300' : 'border-gray-200'}`}>
                        <button
                          type="button"
                          onClick={() => togglePassengerPanel(i)}
                          className={`w-full flex items-center justify-between px-4 py-3 text-start transition-colors ${
                            isOpen ? 'bg-sudan-green-50' : 'bg-gray-50 hover:bg-gray-100'
                          }`}
                        >
                          <div className="flex items-center gap-3 flex-1 min-w-0">
                            <span
                              className={`flex-shrink-0 w-7 h-7 rounded-full flex items-center justify-center text-xs font-bold ${
                                visibleErrors
                                  ? 'bg-red-500 text-white'
                                  : filled
                                    ? 'bg-sudan-green-600 text-white'
                                    : 'bg-white border-2 border-gray-300 text-gray-500'
                              }`}
                            >
                              {visibleErrors ? '!' : filled ? '✓' : i + 1}
                            </span>
                            <div className="min-w-0">
                              <p className="text-sm font-semibold text-sudan-green-900">
                                {AR.counter.passenger} {i + 1} / {passengers.length}
                                {meta && (
                                  <span className="ms-2 text-xs font-normal text-gray-600">
                                    · {AR.counter.coachLabel} {meta.coachNumber} · {AR.counter.seatLabel} {meta.seatNumber}
                                  </span>
                                )}
                                {!meta && (
                                  <span className="ms-2 text-xs font-normal text-red-600">
                                    · {AR.counter.pickSeatFirst}
                                  </span>
                                )}
                              </p>
                              {namePreview && <p className="text-xs text-gray-600 truncate">{namePreview}</p>}
                            </div>
                          </div>
                          {passengers.length > 1 && (
                            <span
                              role="button"
                              tabIndex={0}
                              onClick={(e) => { e.stopPropagation(); removePassenger(i); }}
                              onKeyDown={(e) => { if (e.key === 'Enter' || e.key === ' ') { e.stopPropagation(); removePassenger(i); } }}
                              className="ms-2 text-xs text-red-600 cursor-pointer"
                            >
                              {AR.common.delete}
                            </span>
                          )}
                        </button>

                        {isOpen && (
                          <div className="px-4 py-4 space-y-4 border-t border-gray-200">
                            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                              <div>
                                <label className="block text-sm font-medium text-gray-700 mb-2">{AR.counter.fullNameAr}</label>
                                <input
                                  type="text"
                                  value={ps.passenger.fullNameAr ?? ''}
                                  onChange={(e) => updatePassengerInfo(i, 'fullNameAr', e.target.value)}
                                  onBlur={() => markTouched(i, 'fullNameAr')}
                                  className={`${baseInput} ${errClass('fullNameAr')}`}
                                  required
                                />
                                {errorFor(i, 'fullNameAr') && <p className="text-xs text-red-600 mt-1">{errorFor(i, 'fullNameAr')}</p>}
                              </div>
                              <div>
                                <label className="block text-sm font-medium text-gray-700 mb-2">{AR.counter.fullNameEn}</label>
                                <input
                                  type="text"
                                  value={ps.passenger.fullNameEn ?? ''}
                                  onChange={(e) => updatePassengerInfo(i, 'fullNameEn', e.target.value)}
                                  onBlur={() => markTouched(i, 'fullNameEn')}
                                  className={`${baseInput} ${errClass('fullNameEn')}`}
                                  required
                                />
                                {errorFor(i, 'fullNameEn') && <p className="text-xs text-red-600 mt-1">{errorFor(i, 'fullNameEn')}</p>}
                              </div>
                            </div>
                            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                              <div>
                                <label className="block text-sm font-medium text-gray-700 mb-2">{AR.counter.idPassport}</label>
                                <input
                                  type="text"
                                  value={ps.passenger.idNumber ?? ''}
                                  onChange={(e) => updatePassengerInfo(i, 'idNumber', e.target.value)}
                                  onBlur={() => markTouched(i, 'idNumber')}
                                  className={`${baseInput} ${errClass('idNumber')}`}
                                  required
                                />
                                {errorFor(i, 'idNumber') && <p className="text-xs text-red-600 mt-1">{errorFor(i, 'idNumber')}</p>}
                              </div>
                              <div>
                                <label className="block text-sm font-medium text-gray-700 mb-2">{AR.counter.birthDate}</label>
                                <input
                                  type="date"
                                  value={ps.passenger.birthDate ?? ''}
                                  onChange={(e) => updatePassengerInfo(i, 'birthDate', e.target.value)}
                                  onBlur={() => markTouched(i, 'birthDate')}
                                  className={`${baseInput} ${errClass('birthDate')}`}
                                  required
                                />
                                {errorFor(i, 'birthDate') && <p className="text-xs text-red-600 mt-1">{errorFor(i, 'birthDate')}</p>}
                              </div>
                            </div>
                            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                              <div>
                                <label className="block text-sm font-medium text-gray-700 mb-2">{AR.counter.gender}</label>
                                <div className="flex gap-4 pt-2">
                                  <label className="flex items-center gap-2">
                                    <input
                                      type="radio"
                                      name={`gender-${i}`}
                                      value="male"
                                      checked={(ps.passenger.gender ?? 'male') === 'male'}
                                      onChange={() => updatePassengerInfo(i, 'gender', 'male')}
                                    />
                                    {AR.counter.male}
                                  </label>
                                  <label className="flex items-center gap-2">
                                    <input
                                      type="radio"
                                      name={`gender-${i}`}
                                      value="female"
                                      checked={ps.passenger.gender === 'female'}
                                      onChange={() => updatePassengerInfo(i, 'gender', 'female')}
                                    />
                                    {AR.counter.female}
                                  </label>
                                </div>
                              </div>
                              <div>
                                <label className="block text-sm font-medium text-gray-700 mb-2">{AR.counter.nationality}</label>
                                <select
                                  value={ps.passenger.nationality ?? 'Sudan'}
                                  onChange={(e) => updatePassengerInfo(i, 'nationality', e.target.value)}
                                  className={`${baseInput} ${errClass('nationality')}`}
                                >
                                  <option value="Sudan">{AR.counter.natSudan}</option>
                                  <option value="Egypt">{AR.counter.natEgypt}</option>
                                  <option value="Ethiopia">{AR.counter.natEthiopia}</option>
                                  <option value="Eritrea">{AR.counter.natEritrea}</option>
                                  <option value="Chad">{AR.counter.natChad}</option>
                                  <option value="Libya">{AR.counter.natLibya}</option>
                                  <option value="South Sudan">{AR.counter.natSouthSudan}</option>
                                  <option value="Other">{AR.counter.natOther}</option>
                                </select>
                              </div>
                            </div>
                            <div>
                              <label className="block text-sm font-medium text-gray-700 mb-2">{AR.counter.phoneLabel}</label>
                              <input
                                type="tel"
                                value={ps.passenger.phone ?? ''}
                                onChange={(e) => updatePassengerInfo(i, 'phone', e.target.value)}
                                onBlur={() => markTouched(i, 'phone')}
                                className={`${baseInput} ${errClass('phone')}`}
                                required
                              />
                              {errorFor(i, 'phone') && <p className="text-xs text-red-600 mt-1">{errorFor(i, 'phone')}</p>}
                            </div>
                            <div>
                              <label className="block text-sm font-medium text-gray-700 mb-2">{AR.counter.emailOptional}</label>
                              <input
                                type="email"
                                value={ps.passenger.email ?? ''}
                                onChange={(e) => updatePassengerInfo(i, 'email', e.target.value)}
                                onBlur={() => markTouched(i, 'email')}
                                className={`${baseInput} ${errClass('email')}`}
                              />
                              {errorFor(i, 'email') && <p className="text-xs text-red-600 mt-1">{errorFor(i, 'email')}</p>}
                            </div>

                            {/* Jump to next-passenger panel — same convenience as the customer flow. */}
                            {i < passengers.length - 1 && (
                              <div className="pt-2">
                                <button
                                  type="button"
                                  onClick={() => setExpandedPassengerIdx(i + 1)}
                                  className="px-4 py-2 text-sm border border-sudan-green-300 text-sudan-green-700 rounded-lg hover:bg-sudan-green-50"
                                >
                                  {AR.counter.nextPassenger}
                                </button>
                              </div>
                            )}
                          </div>
                        )}
                      </div>
                    );
                  })}
                </div>

                {showAllErrors && anyValidationErrors && (
                  <p className="text-sm text-red-600 mt-3 text-center">{AR.counter.fixErrors}</p>
                )}
              </div>
              <button className="admin-button-secondary" onClick={addPassenger}>+ {AR.common.add}</button>

              {/* Counter sales are cash-only — no payment-method picker. */}
              <div className="mt-3 text-sm text-gray-600">{AR.counter.paymentCash}</div>

              <div className="flex justify-between">
                <button className="admin-button-secondary" onClick={() => setStep(2)}>{AR.common.cancel}</button>
                <button className="admin-button" onClick={submit} disabled={submitting}>
                  {submitting ? AR.common.processing : AR.counter.confirmCreate}
                </button>
              </div>
            </>
          )}
        </div>
      )}
    </div>
  );
};

export default CounterBookingPage;
