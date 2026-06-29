import { useCallback, useEffect, useMemo, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { User, CreditCard, Check, ArrowLeft, Train as TrainIcon, MapPin, Loader2, ChevronDown, UserCheck } from 'lucide-react';
import QRCode from 'react-qr-code';
import { useLanguage } from '../contexts/LanguageContext';
import { useAuth } from '../contexts/AuthContext';
import { authApi, catalogApi } from '../services/api';
import { bookingApi } from '../services/bookingApi';
import { formatDateSafe, formatTimeSafe } from '../utils/dateUtils';
import type { AvailableSeatDto, BookingDto, CoachSeatsDto, FareDto, SegmentSeatsDto } from '../types/api';

interface TripContext {
  id: number;
  trainName: string;
  trainNumber: string;
  routeId: number;
  departureISO: string;
  arrivalISO: string;
  availableSeats: number;
  price: number;
  currency: string;
  coachClassId: number;
}

interface BookingState {
  trip: TripContext;
  originName: string;
  destinationName: string;
  boardingStationId: number;
  alightingStationId: number;
  passengers: number;
  coachClass: string;
  coachClassId: number;
}

interface PassengerInfo {
  fullNameAr: string;
  fullNameEn: string;
  idNumber: string;
  nationality: string;
  email: string;
  phone: string;
  gender: 'male' | 'female';
  birthDate: string;
}

const EMPTY_PASSENGER: PassengerInfo = {
  fullNameAr: '',
  fullNameEn: '',
  idNumber: '',
  nationality: 'Sudan',
  email: '',
  phone: '',
  gender: 'male',
  birthDate: '',
};

// Nationality values that exist as <option>s in the form. Profile/booking values
// outside this set are ignored so the <select> never holds an invalid value.
const NATIONALITY_OPTIONS = new Set([
  'Sudan', 'Egypt', 'Ethiopia', 'Eritrea', 'Chad', 'Libya', 'South Sudan', 'Other',
]);

type PassengerField = keyof PassengerInfo;
type PassengerErrors = Partial<Record<PassengerField, string>>;

// Client-side validator. Returns one error message per field (or none).
// Server still re-validates — these are just for fast feedback.
function validatePassenger(p: PassengerInfo, t: (k: string) => string): PassengerErrors {
  const errs: PassengerErrors = {};

  // Arabic-only letters + spaces (Arabic Unicode block).
  if (!p.fullNameAr.trim()) errs.fullNameAr = t('validation.required');
  else if (!/^[؀-ۿ\s]+$/.test(p.fullNameAr.trim())) errs.fullNameAr = t('validation.arabic.only');

  // Latin letters + spaces.
  if (!p.fullNameEn.trim()) errs.fullNameEn = t('validation.required');
  else if (!/^[A-Za-z\s]+$/.test(p.fullNameEn.trim())) errs.fullNameEn = t('validation.english.only');

  // ID/passport — accept alphanumerics, 5-30 chars.
  if (!p.idNumber.trim()) errs.idNumber = t('validation.required');
  else if (!/^[A-Za-z0-9]{5,30}$/.test(p.idNumber.trim())) errs.idNumber = t('validation.id.format');

  // Birth date: required, in the past, not >120 years ago.
  if (!p.birthDate) {
    errs.birthDate = t('validation.required');
  } else {
    const bd = new Date(p.birthDate);
    if (isNaN(bd.getTime())) {
      errs.birthDate = t('validation.date.invalid');
    } else {
      const now = new Date();
      if (bd > now) errs.birthDate = t('validation.birthdate.future');
      else {
        const years = now.getFullYear() - bd.getFullYear();
        if (years > 120) errs.birthDate = t('validation.birthdate.too.old');
      }
    }
  }

  // Phone: optional leading "+", 8-15 digits.
  if (!p.phone.trim()) errs.phone = t('validation.required');
  else if (!/^\+?[0-9]{8,15}$/.test(p.phone.replace(/\s/g, ''))) errs.phone = t('validation.phone.format');

  // Email is optional; only checked when present.
  if (p.email.trim() && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(p.email.trim())) {
    errs.email = t('validation.email.format');
  }

  return errs;
}

// UI value → backend PaymentMethod enum
// 0=Cash, 1=CreditCard, 2=DebitCard, 3=BankTransfer, 4=MobilePayment
function paymentMethodToId(_m: string): number {
  return 1; // Visa-only checkout always sends CreditCard
}

interface CardForm {
  cardholderName: string;
  cardNumber: string;
  cardExpiry: string;
  cardCvv: string;
}

type CardField = keyof CardForm;
type CardErrors = Partial<Record<CardField, string>>;

const EMPTY_CARD: CardForm = {
  cardholderName: '',
  cardNumber: '',
  cardExpiry: '',
  cardCvv: '',
};

function validateCard(c: CardForm, t: (k: string) => string): CardErrors {
  const errs: CardErrors = {};

  if (!c.cardholderName.trim()) errs.cardholderName = t('validation.required');
  else if (!/^[A-Za-z\s]+$/.test(c.cardholderName.trim())) errs.cardholderName = t('validation.english.only');

  const digits = c.cardNumber.replace(/\D/g, '');
  if (!digits) errs.cardNumber = t('validation.required');
  else if (!digits.startsWith('4')) errs.cardNumber = t('validation.card.visa.only');
  else if (digits.length !== 16) errs.cardNumber = t('validation.card.number');

  if (!c.cardExpiry.trim()) {
    errs.cardExpiry = t('validation.required');
  } else {
    const expMatch = c.cardExpiry.trim().match(/^(\d{2})\/(\d{2})$/);
    if (!expMatch) {
      errs.cardExpiry = t('validation.card.expiry');
    } else {
      const mm = parseInt(expMatch[1], 10);
      const yy = parseInt(expMatch[2], 10);
      if (mm < 1 || mm > 12) {
        errs.cardExpiry = t('validation.card.expiry');
      } else {
        const expEnd = new Date(2000 + yy, mm, 0);
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        if (expEnd < today) errs.cardExpiry = t('validation.card.expired');
      }
    }
  }

  if (!c.cardCvv.trim()) errs.cardCvv = t('validation.required');
  else if (!/^\d{3}$/.test(c.cardCvv.trim())) errs.cardCvv = t('validation.card.cvv');

  return errs;
}

function formatCardNumber(raw: string): string {
  const digits = raw.replace(/\D/g, '').slice(0, 16);
  return digits.replace(/(\d{4})(?=\d)/g, '$1 ').trim();
}

function formatCardExpiry(raw: string): string {
  const digits = raw.replace(/\D/g, '').slice(0, 4);
  if (digits.length <= 2) return digits;
  return `${digits.slice(0, 2)}/${digits.slice(2)}`;
}

// "First" / "Second" / "Third" → 1/2/3
function coachClassNameToId(name: string | undefined): number {
  switch (name) {
    case 'First': return 1;
    case 'Third': return 3;
    case 'Second':
    default: return 2;
  }
}

export default function BookingPage() {
  const location = useLocation();
  const navigate = useNavigate();
  const { t } = useLanguage();
  const { isAuthenticated } = useAuth();

  // "Use my data" prefill (first passenger only).
  const [prefilling, setPrefilling] = useState(false);
  const [prefillError, setPrefillError] = useState('');

  const state = location.state as BookingState | null;
  const trip = state?.trip;

  // Number of tickets the customer is buying. Comes from the Homepage search.
  // Each ticket = one seat + one passenger's data. Clamp to a sane range.
  const passengerCount = Math.max(1, Math.min(20, state?.passengers ?? 1));

  const [currentStep, setCurrentStep] = useState(1);

  // Selected seats in the order the customer clicked them. The i-th seat is
  // associated with the i-th passenger in step-1's forms.
  const [selectedSeats, setSelectedSeats] = useState<AvailableSeatDto[]>([]);

  // Class filter — '' = any. Drives which coaches are visible in step 2.
  const [chosenClass, setChosenClass] = useState<'' | '1' | '2' | '3'>('');

  const [cardForm, setCardForm] = useState<CardForm>({ ...EMPTY_CARD });
  const [cardTouched, setCardTouched] = useState<Set<CardField>>(new Set());
  const [booking, setBooking] = useState<BookingDto | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  // One PassengerInfo per ticket. Initialized to N empty entries on mount.
  const [passengers, setPassengers] = useState<PassengerInfo[]>(() =>
    Array.from({ length: passengerCount }, () => ({ ...EMPTY_PASSENGER })),
  );
  // Accordion: which passenger panel is currently expanded in step 1.
  // null = all collapsed. Default to passenger 0 expanded.
  const [expandedPassengerIdx, setExpandedPassengerIdx] = useState<number | null>(0);

  // Touched fields per passenger — drives whether to render error messages.
  // Key is "${idx}.${field}". Becomes true on blur OR when user tries to submit
  // step 1 while invalid (we mark every field touched at that point).
  const [touched, setTouched] = useState<Set<string>>(new Set());
  const markTouched = (idx: number, field: PassengerField) =>
    setTouched((prev) => {
      const k = `${idx}.${field}`;
      if (prev.has(k)) return prev;
      const next = new Set(prev);
      next.add(k);
      return next;
    });
  const wasTouched = (idx: number, field: PassengerField) => touched.has(`${idx}.${field}`);
  // Keep the array length in sync if the user navigates back/forward with a
  // different count.
  useEffect(() => {
    setPassengers((prev) => {
      if (prev.length === passengerCount) return prev;
      const next = prev.slice(0, passengerCount);
      while (next.length < passengerCount) next.push({ ...EMPTY_PASSENGER });
      return next;
    });
  }, [passengerCount]);

  const updatePassenger = (idx: number, patch: Partial<PassengerInfo>) =>
    setPassengers((prev) => prev.map((p, i) => (i === idx ? { ...p, ...patch } : p)));

  // Clear touched markers for given fields of a passenger, so freshly prefilled
  // values don't immediately render stale validation errors.
  const clearTouched = (idx: number, fields: PassengerField[]) =>
    setTouched((prev) => {
      const next = new Set(prev);
      for (const f of fields) next.delete(`${idx}.${f}`);
      return next;
    });

  // "Use my data": fill the first passenger from the logged-in user's profile
  // and their most recent past booking. Only non-empty values overwrite fields.
  const handleUseMyData = async () => {
    setPrefilling(true);
    setPrefillError('');
    try {
      const [profile, bookings] = await Promise.all([
        authApi.getProfile().catch(() => null),
        bookingApi.getMyBookings().catch(() => null),
      ]);

      const patch: Partial<PassengerInfo> = {};

      if (profile) {
        const fullNameEn = `${profile.firstName ?? ''} ${profile.lastName ?? ''}`.trim();
        if (fullNameEn) patch.fullNameEn = fullNameEn;
        if (profile.email) patch.email = profile.email;
        if (profile.phoneNumber) patch.phone = profile.phoneNumber;
        if (profile.nationality && NATIONALITY_OPTIONS.has(profile.nationality)) {
          patch.nationality = profile.nationality;
        }
      }

      // Most recent booking's primary passenger carries id/Arabic name/gender.
      const latest = bookings && bookings.length > 0
        ? [...bookings].sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())[0]
        : null;
      const prev = latest?.passengers?.[0]?.passenger ?? latest?.passenger ?? null;
      if (prev) {
        if (prev.idNumber) patch.idNumber = prev.idNumber;
        if (prev.fullNameAr) patch.fullNameAr = prev.fullNameAr;
        if (!patch.fullNameEn && prev.fullNameEn) patch.fullNameEn = prev.fullNameEn;
        const g = prev.gender?.toLowerCase();
        if (g === 'male' || g === 'female') patch.gender = g;
        if (prev.nationality && NATIONALITY_OPTIONS.has(prev.nationality) && !patch.nationality) {
          patch.nationality = prev.nationality;
        }
        if (!patch.phone && prev.phone) patch.phone = prev.phone;
        if (!patch.email && prev.email) patch.email = prev.email;
      }

      if (Object.keys(patch).length === 0) {
        setPrefillError(t('use.my.data.empty'));
        return;
      }

      updatePassenger(0, patch);
      clearTouched(0, Object.keys(patch) as PassengerField[]);
      setExpandedPassengerIdx(0);
    } catch {
      setPrefillError(t('use.my.data.error'));
    } finally {
      setPrefilling(false);
    }
  };

  // Accordion: toggle the panel. Auto-collapse the previously-open one so only
  // one is open at a time.
  const togglePassengerPanel = (idx: number) =>
    setExpandedPassengerIdx((prev) => (prev === idx ? null : idx));

  // Required-fields check — drives the "filled" check mark on the panel header.
  const isPassengerFilled = (p: PassengerInfo) =>
    !!(p.fullNameAr && p.fullNameEn && p.idNumber && p.birthDate && p.phone);

  // Per-passenger validation errors (recomputed each render — passengers state
  // is the source of truth, no separate error state needed).
  const passengerErrors = useMemo(
    () => passengers.map((p) => validatePassenger(p, t)),
    [passengers, t],
  );
  const allPassengersValid = passengerErrors.every((errs) => Object.keys(errs).length === 0);

  const cardErrors = useMemo(() => validateCard(cardForm, t), [cardForm, t]);
  const cardValid = Object.keys(cardErrors).length === 0;
  const markCardTouched = (field: CardField) =>
    setCardTouched((prev) => (prev.has(field) ? prev : new Set(prev).add(field)));
  const cardErrorFor = (field: CardField): string | null =>
    cardErrors[field] && cardTouched.has(field) ? cardErrors[field]! : null;
  const cardFieldClass = (field: CardField) => {
    if (cardErrors[field] && cardTouched.has(field)) {
      return `${inputClass} border-red-400 focus:ring-red-500 focus:border-red-500`;
    }
    return inputClass;
  };
  const markAllCardTouched = () => setCardTouched(new Set(Object.keys(EMPTY_CARD) as CardField[]));

  // Mark every field on every passenger as touched. Called when the user hits
  // "Next" with validation errors so all errors render at once.
  const markAllTouched = () => {
    const all = new Set<string>();
    const fields: PassengerField[] = ['fullNameAr', 'fullNameEn', 'idNumber', 'birthDate', 'phone', 'email'];
    for (let i = 0; i < passengers.length; i++) {
      for (const f of fields) all.add(`${i}.${f}`);
    }
    setTouched(all);
  };

  const handleStep1Submit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!allPassengersValid) {
      markAllTouched();
      const firstInvalid = passengerErrors.findIndex((errs) => Object.keys(errs).length > 0);
      if (firstInvalid >= 0) setExpandedPassengerIdx(firstInvalid);
      return;
    }
    setCurrentStep(2);
  };

  // Classes for an input field that may be invalid — adds a red border when the
  // field has been touched (or step-1 submit was attempted) AND has an error.
  const fieldClassFor = (idx: number, field: PassengerField) => {
    const err = passengerErrors[idx]?.[field];
    if (err && wasTouched(idx, field)) {
      return `${inputClass} border-red-400 focus:ring-red-500 focus:border-red-500`;
    }
    return inputClass;
  };

  const errorFor = (idx: number, field: PassengerField): string | null => {
    const err = passengerErrors[idx]?.[field];
    return err && wasTouched(idx, field) ? err : null;
  };

  const [seatsMap, setSeatsMap] = useState<SegmentSeatsDto | null>(null);
  const [seatsLoading, setSeatsLoading] = useState(false);
  const [seatsError, setSeatsError] = useState('');

  // Which classes have BOTH at least one available seat AND a configured fare.
  const [availableClassIds, setAvailableClassIds] = useState<Set<'1' | '2' | '3'>>(new Set());

  // Fare preview for ONE seat (per the chosen / inferred class). Total =
  // selectedSeats.length × this. Backend resolves it once the chosen class
  // (or the first selected seat's class) is known.
  const [previewFare, setPreviewFare] = useState<FareDto | null>(null);

  const fetchSeats = useCallback(async () => {
    if (!trip || !state) return;
    setSeatsLoading(true);
    setSeatsError('');
    try {
      const result = await catalogApi.getSegmentSeats(trip.id, state.boardingStationId, state.alightingStationId);
      setSeatsMap(result);
      // Drop any selected seat that's no longer available.
      setSelectedSeats((prev) =>
        prev.filter((s) =>
          result.coaches.flatMap((c) => c.seats).some((nx) => nx.id === s.id && nx.isAvailable),
        ),
      );
    } catch (err) {
      setSeatsError(err instanceof Error ? err.message : t('error'));
    } finally {
      setSeatsLoading(false);
    }
  }, [trip, state, t]);

  useEffect(() => {
    if (currentStep === 2 && !seatsMap && !seatsLoading) {
      fetchSeats();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [currentStep]);

  // Probe each coach class present in seatsMap for a configured fare.
  useEffect(() => {
    if (!seatsMap || !trip || !state) {
      setAvailableClassIds(new Set());
      return;
    }
    const classNameToId: Record<string, '1' | '2' | '3' | undefined> = {
      First: '1', Second: '2', Third: '3',
    };
    const candidates = new Set<'1' | '2' | '3'>();
    for (const coach of seatsMap.coaches) {
      const id = classNameToId[coach.class];
      if (id && coach.seats.some((s) => s.isAvailable)) candidates.add(id);
    }
    if (candidates.size === 0) {
      setAvailableClassIds(new Set());
      return;
    }
    let cancelled = false;
    Promise.all(
      Array.from(candidates).map((c) =>
        catalogApi
          .getApplicableFare(trip.id, state.boardingStationId, state.alightingStationId, Number(c))
          .then((fare) => (fare ? c : null))
          .catch(() => null),
      ),
    ).then((rows) => {
      if (cancelled) return;
      const set = new Set<'1' | '2' | '3'>();
      for (const r of rows) if (r) set.add(r);
      setAvailableClassIds(set);
      if (chosenClass && !set.has(chosenClass)) {
        setChosenClass('');
        setSelectedSeats([]);
      }
    });
    return () => { cancelled = true; };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [seatsMap, trip, state]);

  // Fare preview: prefer the first selected seat's coach class → chosenClass → undefined (cheapest).
  useEffect(() => {
    if (!trip || !state) {
      setPreviewFare(null);
      return;
    }
    let cancelled = false;
    let inferredClass: number | undefined;
    if (selectedSeats.length > 0 && seatsMap) {
      const firstSeat = selectedSeats[0];
      const coach = seatsMap.coaches.find((c) => c.seats.some((s) => s.id === firstSeat.id));
      if (coach) inferredClass = coachClassNameToId(coach.class);
    }
    const fareClass = inferredClass ?? (chosenClass ? Number(chosenClass) : undefined);
    catalogApi
      .getApplicableFare(trip.id, state.boardingStationId, state.alightingStationId, fareClass)
      .then((fare) => { if (!cancelled) setPreviewFare(fare); })
      .catch(() => { if (!cancelled) setPreviewFare(null); });
    return () => { cancelled = true; };
  }, [trip, state, selectedSeats, chosenClass, seatsMap]);

  const previewBreakdown = booking?.breakdown ?? previewFare?.breakdown ?? null;
  const unitPrice = previewBreakdown?.total ?? previewFare?.finalPrice ?? trip?.price ?? 0;
  const total = booking?.total ?? unitPrice * (selectedSeats.length || passengerCount);

  const steps = [
    { id: 1, name: t('passenger.info'), icon: User },
    { id: 2, name: t('seat.selection'), icon: TrainIcon },
    { id: 3, name: t('payment'), icon: CreditCard },
    { id: 4, name: t('confirm'), icon: Check },
  ];

  // Helper: find which coach a seat belongs to.
  const coachOfSeat = useCallback((seatId: number): CoachSeatsDto | null => {
    if (!seatsMap) return null;
    return seatsMap.coaches.find((c) => c.seats.some((s) => s.id === seatId)) ?? null;
  }, [seatsMap]);

  const visibleCoaches: CoachSeatsDto[] = useMemo(() => {
    if (!seatsMap?.coaches.length) return [];
    if (!chosenClass) return seatsMap.coaches;
    const wanted = chosenClass === '1' ? 'First' : chosenClass === '2' ? 'Second' : 'Third';
    return seatsMap.coaches.filter((c) => c.class === wanted);
  }, [seatsMap, chosenClass]);

  if (!trip || !state) {
    return (
      <div className="min-h-[60vh] flex items-center justify-center">
        <div className="text-center">
          <TrainIcon className="h-12 w-12 text-gray-400 mx-auto mb-4" />
          <p className="text-gray-600 mb-4">{t('no.trains')}</p>
          <button onClick={() => navigate('/search')} className="bg-sudan-green-600 text-white px-4 py-2 rounded-lg hover:bg-sudan-green-700">
            {t('search')}
          </button>
        </div>
      </div>
    );
  }

  const toggleSeat = (seat: AvailableSeatDto) => {
    if (!seat.isAvailable) return;
    setSelectedSeats((prev) => {
      const existingIdx = prev.findIndex((s) => s.id === seat.id);
      if (existingIdx >= 0) {
        // Deselect.
        return prev.filter((s) => s.id !== seat.id);
      }
      // Hit cap — silently refuse extra picks.
      if (prev.length >= passengerCount) return prev;
      return [...prev, seat];
    });
  };

  const autoFillSeats = () => {
    if (!seatsMap) return;
    const allFree = visibleCoaches.flatMap((c) => c.seats).filter((s) => s.isAvailable);
    if (!allFree.length) return;
    // Take the first N free seats from the visible coach list.
    const remaining = passengerCount - selectedSeats.length;
    if (remaining <= 0) return;
    const alreadyPickedIds = new Set(selectedSeats.map((s) => s.id));
    const additions = allFree.filter((s) => !alreadyPickedIds.has(s.id)).slice(0, remaining);
    setSelectedSeats((prev) => [...prev, ...additions]);
  };

  const handlePay = async (e: React.FormEvent) => {
    e.preventDefault();
    if (selectedSeats.length !== passengerCount) {
      setError(t('select.all.seats') || 'Please select a seat for every passenger.');
      return;
    }
    if (!cardValid) {
      markAllCardTouched();
      return;
    }
    const digits = cardForm.cardNumber.replace(/\D/g, '');
    setError('');
    setSubmitting(true);
    try {
      const payloadPassengers = selectedSeats.map((seat, i) => {
        const coach = coachOfSeat(seat.id);
        const coachClass = coach?.class ? coachClassNameToId(coach.class) : (state.coachClassId || trip.coachClassId);
        const p = passengers[i] ?? EMPTY_PASSENGER;
        return {
          seatId: seat.id,
          coachClass,
          passenger: {
            fullNameEn: p.fullNameEn,
            fullNameAr: p.fullNameAr,
            idNumber: p.idNumber,
            birthDate: p.birthDate,
            gender: p.gender,
            nationality: p.nationality,
            phone: p.phone,
            email: p.email || undefined,
          },
        };
      });
      const created = await bookingApi.createBooking({
        tripId: trip.id,
        boardingStationId: state.boardingStationId,
        alightingStationId: state.alightingStationId,
        paymentMethod: paymentMethodToId('card'),
        cardLast4: digits.slice(-4),
        passengers: payloadPassengers,
      });
      setBooking(created);
      setCurrentStep(4);
    } catch (err) {
      const msg = err instanceof Error ? err.message : t('error');
      if (/declined|payment/i.test(msg)) {
        setError(t('payment.declined'));
      } else if (/422|conflict|unavailable|seat/i.test(msg)) {
        setError(t('seat.just.taken') || 'A seat was just taken — please choose another.');
        setSelectedSeats([]);
        await fetchSeats();
        setCurrentStep(2);
      } else {
        setError(msg);
      }
    } finally {
      setSubmitting(false);
    }
  };

  const inputClass =
    'w-full px-3 sm:px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-sudan-green-500 focus:border-sudan-green-500 text-sm sm:text-base';

  return (
    <div className="min-h-screen bg-gray-50 py-4 sm:py-8">
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Progress */}
        <div className="bg-white rounded-lg shadow-md p-4 sm:p-6 mb-4 sm:mb-8">
          <div className="flex items-center justify-between overflow-x-auto">
            {steps.map((step, index) => (
              <div key={step.id} className="flex items-center flex-shrink-0">
                <div className={`flex items-center justify-center w-10 h-10 rounded-full ${currentStep >= step.id ? 'bg-sudan-green-600 text-white' : 'bg-gray-200 text-gray-600'}`}>
                  <step.icon className="h-5 w-5" />
                </div>
                <span className={`ml-2 rtl:ml-0 rtl:mr-2 font-medium text-sm sm:text-base hidden sm:inline ${currentStep >= step.id ? 'text-sudan-green-600' : 'text-gray-500'}`}>
                  {step.name}
                </span>
                {index < steps.length - 1 && (
                  <div className={`w-8 sm:w-16 h-0.5 ml-4 sm:ml-8 rtl:ml-0 rtl:mr-4 sm:rtl:mr-8 ${currentStep > step.id ? 'bg-sudan-green-600' : 'bg-gray-200'}`} />
                )}
              </div>
            ))}
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <div className="lg:col-span-2">
            <div className="bg-white rounded-lg shadow-md p-4 sm:p-6">
              {error && <p className="bg-red-50 text-red-600 text-sm rounded-lg p-3 mb-4">{error}</p>}

              {/* STEP 1 — N passenger forms */}
              {currentStep === 1 && (
                <div>
                  <h2 className="text-xl sm:text-2xl font-bold text-gray-900 mb-2">{t('passenger.info')}</h2>
                  <p className="text-sm text-gray-600 mb-4 sm:mb-6">{t('passengers.count.note').replace('{n}', String(passengerCount))}</p>
                  <form onSubmit={handleStep1Submit} className="space-y-3" noValidate>
                    {passengers.map((p, i) => {
                      const isOpen = expandedPassengerIdx === i;
                      const filled = isPassengerFilled(p);
                      const errs = passengerErrors[i];
                      const hasErrors = Object.keys(errs).length > 0;
                      // Are any errors *visible* (touched)?
                      const showingErrors = Object.keys(errs).some((f) => wasTouched(i, f as PassengerField));
                      const namePreview = p.fullNameAr || p.fullNameEn;
                      return (
                        <div key={i} className={`border rounded-lg overflow-hidden bg-white ${showingErrors ? 'border-red-300' : 'border-gray-200'}`}>
                          {/* Accordion header — always visible; click toggles the body. */}
                          <button
                            type="button"
                            onClick={() => togglePassengerPanel(i)}
                            className={`w-full flex items-center justify-between px-4 sm:px-5 py-3 text-start transition-colors ${
                              isOpen ? 'bg-sudan-green-50' : 'bg-gray-50 hover:bg-gray-100'
                            }`}
                            aria-expanded={isOpen}
                            aria-controls={`passenger-panel-${i}`}
                          >
                            <div className="flex items-center gap-3 flex-1 min-w-0">
                              <span className={`flex-shrink-0 w-7 h-7 rounded-full flex items-center justify-center text-xs font-bold ${
                                showingErrors
                                  ? 'bg-red-500 text-white'
                                  : filled && !hasErrors
                                    ? 'bg-sudan-green-600 text-white'
                                    : 'bg-white border-2 border-gray-300 text-gray-500'
                              }`}>
                                {showingErrors ? '!' : filled && !hasErrors ? <Check className="h-3.5 w-3.5" /> : i + 1}
                              </span>
                              <div className="min-w-0">
                                <p className="text-sm sm:text-base font-semibold text-sudan-green-900">
                                  {t('passenger')} {i + 1} / {passengerCount}
                                </p>
                                {namePreview && (
                                  <p className="text-xs text-gray-600 truncate">{namePreview}</p>
                                )}
                              </div>
                            </div>
                            <ChevronDown
                              className={`h-5 w-5 text-gray-400 flex-shrink-0 transition-transform ${isOpen ? 'rotate-180' : ''}`}
                            />
                          </button>

                          {/* Accordion body — form fields. */}
                          {isOpen && (
                            <div id={`passenger-panel-${i}`} className="px-4 sm:px-5 py-4 space-y-4 border-t border-gray-200">
                              {i === 0 && isAuthenticated && (
                                <div>
                                  <button
                                    type="button"
                                    onClick={handleUseMyData}
                                    disabled={prefilling}
                                    className="inline-flex items-center gap-2 px-4 py-2 text-sm rounded-lg border border-sudan-green-300 text-sudan-green-700 hover:bg-sudan-green-50 disabled:opacity-60"
                                  >
                                    {prefilling ? <Loader2 className="h-4 w-4 animate-spin" /> : <UserCheck className="h-4 w-4" />}
                                    {prefilling ? t('use.my.data.loading') : t('use.my.data')}
                                  </button>
                                  {prefillError && <p className="text-xs text-red-600 mt-2">{prefillError}</p>}
                                </div>
                              )}
                              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                                <div>
                                  <label className="block text-sm font-medium text-gray-700 mb-2">{t('full.name.arabic')}</label>
                                  <input
                                    type="text"
                                    value={p.fullNameAr}
                                    onChange={(e) => updatePassenger(i, { fullNameAr: e.target.value })}
                                    onBlur={() => markTouched(i, 'fullNameAr')}
                                    className={fieldClassFor(i, 'fullNameAr')}
                                    required
                                  />
                                  {errorFor(i, 'fullNameAr') && <p className="text-xs text-red-600 mt-1">{errorFor(i, 'fullNameAr')}</p>}
                                </div>
                                <div>
                                  <label className="block text-sm font-medium text-gray-700 mb-2">{t('full.name.english')}</label>
                                  <input
                                    type="text"
                                    value={p.fullNameEn}
                                    onChange={(e) => updatePassenger(i, { fullNameEn: e.target.value })}
                                    onBlur={() => markTouched(i, 'fullNameEn')}
                                    className={fieldClassFor(i, 'fullNameEn')}
                                    required
                                  />
                                  {errorFor(i, 'fullNameEn') && <p className="text-xs text-red-600 mt-1">{errorFor(i, 'fullNameEn')}</p>}
                                </div>
                              </div>
                              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                                <div>
                                  <label className="block text-sm font-medium text-gray-700 mb-2">{t('id.passport.number')}</label>
                                  <input
                                    type="text"
                                    value={p.idNumber}
                                    onChange={(e) => updatePassenger(i, { idNumber: e.target.value })}
                                    onBlur={() => markTouched(i, 'idNumber')}
                                    className={fieldClassFor(i, 'idNumber')}
                                    required
                                  />
                                  {errorFor(i, 'idNumber') && <p className="text-xs text-red-600 mt-1">{errorFor(i, 'idNumber')}</p>}
                                </div>
                                <div>
                                  <label className="block text-sm font-medium text-gray-700 mb-2">{t('birth.date')}</label>
                                  <input
                                    type="date"
                                    value={p.birthDate}
                                    onChange={(e) => updatePassenger(i, { birthDate: e.target.value })}
                                    onBlur={() => markTouched(i, 'birthDate')}
                                    className={fieldClassFor(i, 'birthDate')}
                                    required
                                  />
                                  {errorFor(i, 'birthDate') && <p className="text-xs text-red-600 mt-1">{errorFor(i, 'birthDate')}</p>}
                                </div>
                              </div>
                              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                                <div>
                                  <label className="block text-sm font-medium text-gray-700 mb-2">{t('gender')}</label>
                                  <div className="flex space-x-4 rtl:space-x-reverse pt-2">
                                    <label className="flex items-center">
                                      <input type="radio" name={`gender-${i}`} value="male" checked={p.gender === 'male'} onChange={() => updatePassenger(i, { gender: 'male' })} className="mr-2 rtl:mr-0 rtl:ml-2" />
                                      {t('male')}
                                    </label>
                                    <label className="flex items-center">
                                      <input type="radio" name={`gender-${i}`} value="female" checked={p.gender === 'female'} onChange={() => updatePassenger(i, { gender: 'female' })} className="mr-2 rtl:mr-0 rtl:ml-2" />
                                      {t('female')}
                                    </label>
                                  </div>
                                </div>
                                <div>
                                  <label className="block text-sm font-medium text-gray-700 mb-2">{t('nationality')}</label>
                                  <select value={p.nationality} onChange={(e) => updatePassenger(i, { nationality: e.target.value })} className={inputClass}>
                                    <option value="Sudan">{t('sudan')}</option>
                                    <option value="Egypt">{t('egypt')}</option>
                                    <option value="Ethiopia">{t('ethiopia')}</option>
                                    <option value="Eritrea">{t('eritrea')}</option>
                                    <option value="Chad">{t('chad')}</option>
                                    <option value="Libya">{t('libya')}</option>
                                    <option value="South Sudan">{t('south.sudan')}</option>
                                    <option value="Other">{t('other')}</option>
                                  </select>
                                </div>
                              </div>
                              <div>
                                <label className="block text-sm font-medium text-gray-700 mb-2">{t('phone')}</label>
                                <input
                                  type="tel"
                                  value={p.phone}
                                  onChange={(e) => updatePassenger(i, { phone: e.target.value })}
                                  onBlur={() => markTouched(i, 'phone')}
                                  className={fieldClassFor(i, 'phone')}
                                  required
                                />
                                {errorFor(i, 'phone') && <p className="text-xs text-red-600 mt-1">{errorFor(i, 'phone')}</p>}
                              </div>
                              <div>
                                <label className="block text-sm font-medium text-gray-700 mb-2">{t('email')} ({t('optional')})</label>
                                <input
                                  type="email"
                                  value={p.email}
                                  onChange={(e) => updatePassenger(i, { email: e.target.value })}
                                  onBlur={() => markTouched(i, 'email')}
                                  className={fieldClassFor(i, 'email')}
                                />
                                {errorFor(i, 'email') && <p className="text-xs text-red-600 mt-1">{errorFor(i, 'email')}</p>}
                              </div>

                              {/* Convenience: jump to the next passenger panel when there are more. */}
                              {i < passengerCount - 1 && (
                                <div className="pt-2">
                                  <button
                                    type="button"
                                    onClick={() => setExpandedPassengerIdx(i + 1)}
                                    className="w-full sm:w-auto px-4 py-2 text-sm border border-sudan-green-300 text-sudan-green-700 rounded-lg hover:bg-sudan-green-50"
                                  >
                                    {t('next.passenger')}
                                  </button>
                                </div>
                              )}
                            </div>
                          )}
                        </div>
                      );
                    })}

                    {!allPassengersValid && touched.size > 0 && (
                      <p className="text-xs text-red-600 text-center">{t('validation.fix.errors')}</p>
                    )}

                    <button
                      type="submit"
                      className="w-full bg-sudan-green-600 text-white py-3 rounded-lg font-medium hover:bg-sudan-green-700 transition-colors text-sm sm:text-base mt-4 disabled:opacity-60"
                    >
                      {t('next')}
                    </button>
                  </form>
                </div>
              )}

              {/* STEP 2 — seat selection (multi) */}
              {currentStep === 2 && (
                <div>
                  <h2 className="text-xl sm:text-2xl font-bold text-gray-900 mb-2">{t('seat.selection')}</h2>
                  <p className="text-sm text-gray-600 mb-4">
                    {t('seats.picked')} {selectedSeats.length} / {passengerCount}
                  </p>

                  {availableClassIds.size >= 2 && (
                    <div className="mb-4 flex flex-wrap items-center gap-2">
                      <span className="text-sm text-gray-600">{t('class')}:</span>
                      {([
                        { v: '', k: 'any.class', show: true },
                        { v: '1', k: 'first.class', show: availableClassIds.has('1') },
                        { v: '2', k: 'second.class', show: availableClassIds.has('2') },
                        { v: '3', k: 'third.class', show: availableClassIds.has('3') },
                      ] as const).filter((p) => p.show).map(({ v, k }) => (
                        <button
                          key={v || 'any'}
                          type="button"
                          onClick={() => {
                            if (chosenClass === v) return;
                            setChosenClass(v as '' | '1' | '2' | '3');
                            setSelectedSeats([]);
                          }}
                          className={`px-3 py-1.5 rounded-full text-xs sm:text-sm border ${
                            chosenClass === v
                              ? 'bg-sudan-green-600 text-white border-sudan-green-600'
                              : 'bg-white text-gray-700 border-gray-300 hover:bg-gray-50'
                          }`}
                        >
                          {t(k)}
                        </button>
                      ))}
                    </div>
                  )}

                  {seatsLoading ? (
                    <div className="text-center py-12">
                      <Loader2 className="h-8 w-8 animate-spin text-sudan-green-600 mx-auto mb-3" />
                      <p className="text-sm text-gray-600">{t('loading')}</p>
                    </div>
                  ) : seatsError ? (
                    <div className="bg-red-50 text-red-600 rounded-lg p-4 mb-4">
                      {seatsError}
                      <button onClick={fetchSeats} className="block mt-2 text-sm underline">{t('retry') || 'Retry'}</button>
                    </div>
                  ) : !seatsMap || seatsMap.availableCount === 0 ? (
                    <div className="bg-gray-50 rounded-lg p-6 text-center text-gray-600">
                      {t('no.seats.available')}
                    </div>
                  ) : visibleCoaches.length === 0 ? (
                    <div className="bg-gray-50 rounded-lg p-6 text-center text-gray-600">
                      {t('no.coaches.in.class')}
                    </div>
                  ) : (
                    <div className="space-y-4">
                      {visibleCoaches.map((coach) => {
                        const avail = coach.seats.filter((s) => s.isAvailable).length;
                        return (
                          <div key={coach.id} className="bg-gray-50 rounded-lg p-4 sm:p-6">
                            <p className="text-center text-xs text-gray-500 mb-3">
                              {t('coach')} {coach.coachNumber} · {coach.class} · {avail} {t('available')}
                            </p>
                            <div className="grid grid-cols-4 gap-1 sm:gap-2 max-w-xs sm:max-w-md mx-auto">
                              {coach.seats.map((seat) => {
                                const selectedIdx = selectedSeats.findIndex((s) => s.id === seat.id);
                                const isSelected = selectedIdx >= 0;
                                const isAvailable = seat.isAvailable;
                                const atCap = !isSelected && selectedSeats.length >= passengerCount;
                                return (
                                  <button
                                    type="button"
                                    key={seat.id}
                                    onClick={() => toggleSeat(seat)}
                                    disabled={!isAvailable || atCap}
                                    title={seat.isWindow ? t('window.seat') : undefined}
                                    className={`relative w-8 h-8 sm:w-10 sm:h-10 rounded border-2 transition-colors text-[10px] sm:text-xs font-medium ${
                                      isSelected
                                        ? 'bg-sudan-gold-500 border-sudan-gold-500 text-sudan-green-900'
                                        : !isAvailable
                                          ? 'bg-red-200 border-red-300 cursor-not-allowed text-red-700'
                                          : atCap
                                            ? 'bg-white border-gray-200 text-gray-300 cursor-not-allowed'
                                            : 'bg-white border-gray-300 hover:bg-sudan-green-50 hover:border-sudan-green-300'
                                    }`}
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
                        );
                      })}

                      <div className="flex flex-wrap justify-center gap-4">
                        <div className="flex items-center gap-2"><div className="w-4 h-4 bg-white border-2 border-gray-300 rounded"></div><span className="text-xs sm:text-sm">{t('available')}</span></div>
                        <div className="flex items-center gap-2"><div className="w-4 h-4 bg-red-200 border-2 border-red-300 rounded"></div><span className="text-xs sm:text-sm">{t('occupied')}</span></div>
                        <div className="flex items-center gap-2"><div className="w-4 h-4 bg-sudan-gold-500 border-2 border-sudan-gold-500 rounded"></div><span className="text-xs sm:text-sm">{t('selected')}</span></div>
                      </div>

                      {selectedSeats.length > 0 && (
                        <div className="rounded-lg border border-sudan-green-200 bg-sudan-green-50 p-3 space-y-1">
                          <p className="text-sm font-medium text-sudan-green-800 mb-1">{t('selected.seats')}</p>
                          {selectedSeats.map((seat, i) => {
                            const coach = coachOfSeat(seat.id);
                            const passengerName = passengers[i]?.fullNameAr || passengers[i]?.fullNameEn || `${t('passenger')} ${i + 1}`;
                            return (
                              <div key={seat.id} className="text-xs text-sudan-green-900 flex justify-between">
                                <span>{i + 1}. {passengerName}</span>
                                <span>
                                  {seat.seatNumber}{coach ? ` · ${coach.coachNumber}` : ''}
                                </span>
                              </div>
                            );
                          })}
                        </div>
                      )}

                      <div className="text-center">
                        <button
                          type="button"
                          onClick={autoFillSeats}
                          className="text-sudan-green-600 text-sm hover:text-sudan-green-800 transition-colors"
                        >
                          {t('auto.select.seat')}
                        </button>
                      </div>
                    </div>
                  )}

                  <div className="flex flex-col sm:flex-row gap-2 sm:gap-4 mt-4">
                    <button onClick={() => setCurrentStep(1)} className="flex items-center justify-center px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 text-sm sm:text-base">
                      <ArrowLeft className="h-4 w-4 mr-2 rtl:mr-0 rtl:ml-2" />
                      {t('previous')}
                    </button>
                    <button
                      onClick={() => setCurrentStep(3)}
                      disabled={selectedSeats.length !== passengerCount}
                      className="flex-1 bg-sudan-green-600 text-white py-2 rounded-lg font-medium hover:bg-sudan-green-700 disabled:opacity-50 text-sm sm:text-base"
                    >
                      {t('next')}
                    </button>
                  </div>
                </div>
              )}

              {/* STEP 3 — payment */}
              {currentStep === 3 && (
                <div>
                  <h2 className="text-xl sm:text-2xl font-bold text-gray-900 mb-4 sm:mb-6">{t('payment')}</h2>
                  <div className="mb-6">
                    <div className="flex items-center gap-3 p-4 border-2 border-sudan-green-600 rounded-lg bg-sudan-green-50">
                      <div className="bg-white px-3 py-1.5 rounded border border-gray-200 font-bold text-blue-800 tracking-wider text-sm">
                        VISA
                      </div>
                      <div>
                        <p className="font-medium text-sudan-green-900">{t('payment.visa.only')}</p>
                        <p className="text-sm text-gray-600">Visa</p>
                      </div>
                    </div>
                  </div>
                  <form onSubmit={handlePay} className="space-y-4 sm:space-y-6" noValidate>
                    <>
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">{t('cardholder.name')}</label>
                        <input
                          type="text"
                          value={cardForm.cardholderName}
                          onChange={(e) => setCardForm((prev) => ({ ...prev, cardholderName: e.target.value }))}
                          onBlur={() => markCardTouched('cardholderName')}
                          className={cardFieldClass('cardholderName')}
                        />
                        {cardErrorFor('cardholderName') && <p className="text-xs text-red-600 mt-1">{cardErrorFor('cardholderName')}</p>}
                      </div>
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">{t('card.number')}</label>
                        <input
                          type="text"
                          inputMode="numeric"
                          autoComplete="cc-number"
                          placeholder="4111 1111 1111 1111"
                          value={cardForm.cardNumber}
                          onChange={(e) => {
                            const formatted = formatCardNumber(e.target.value);
                            const digits = formatted.replace(/\D/g, '');
                            setCardForm((prev) => ({ ...prev, cardNumber: formatted }));
                          }}
                          onBlur={() => markCardTouched('cardNumber')}
                          className={cardFieldClass('cardNumber')}
                        />
                        {cardErrorFor('cardNumber') && <p className="text-xs text-red-600 mt-1">{cardErrorFor('cardNumber')}</p>}
                      </div>
                      <div className="grid grid-cols-2 gap-3 sm:gap-4">
                        <div>
                          <label className="block text-sm font-medium text-gray-700 mb-2">{t('expiry.date')}</label>
                          <input
                            type="text"
                            inputMode="numeric"
                            autoComplete="cc-exp"
                            placeholder="MM/YY"
                            value={cardForm.cardExpiry}
                            onChange={(e) => setCardForm((prev) => ({ ...prev, cardExpiry: formatCardExpiry(e.target.value) }))}
                            onBlur={() => markCardTouched('cardExpiry')}
                            className={cardFieldClass('cardExpiry')}
                          />
                          {cardErrorFor('cardExpiry') && <p className="text-xs text-red-600 mt-1">{cardErrorFor('cardExpiry')}</p>}
                        </div>
                        <div>
                          <label className="block text-sm font-medium text-gray-700 mb-2">CVV</label>
                          <input
                            type="text"
                            inputMode="numeric"
                            autoComplete="cc-csc"
                            placeholder="123"
                            maxLength={3}
                            value={cardForm.cardCvv}
                            onChange={(e) => setCardForm((prev) => ({ ...prev, cardCvv: e.target.value.replace(/\D/g, '').slice(0, 3) }))}
                            onBlur={() => markCardTouched('cardCvv')}
                            className={cardFieldClass('cardCvv')}
                          />
                          {cardErrorFor('cardCvv') && <p className="text-xs text-red-600 mt-1">{cardErrorFor('cardCvv')}</p>}
                        </div>
                      </div>
                    </>
                    <div className="bg-sudan-green-50 border border-sudan-green-200 rounded-lg p-3">
                      <label className="flex items-center">
                        <input type="checkbox" className="mr-2 rtl:mr-0 rtl:ml-2" required />
                        <span className="text-sm text-sudan-green-800">{t('agree.terms.conditions')}</span>
                      </label>
                    </div>
                    <div className="flex flex-col sm:flex-row gap-2 sm:gap-4">
                      <button type="button" onClick={() => setCurrentStep(2)} className="flex items-center justify-center px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 text-sm sm:text-base">
                        <ArrowLeft className="h-4 w-4 mr-2 rtl:mr-0 rtl:ml-2" />
                        {t('previous')}
                      </button>
                      <button type="submit" disabled={submitting} className="flex-1 bg-sudan-green-600 text-white py-2 rounded-lg font-medium hover:bg-sudan-green-700 disabled:opacity-60 flex items-center justify-center gap-2 text-sm sm:text-base">
                        {submitting && <Loader2 className="h-4 w-4 animate-spin" />}
                        {submitting ? t('processing') : `${t('continue.payment')} · ${Math.round(total)} ${t('sdg')}`}
                      </button>
                    </div>
                  </form>
                </div>
              )}

              {/* STEP 4 — confirmation, one card per ticket */}
              {currentStep === 4 && booking && (
                <div>
                  <div className="text-center mb-6">
                    <div className="bg-green-100 rounded-full w-16 h-16 mx-auto mb-4 flex items-center justify-center">
                      <Check className="h-8 w-8 text-green-600" />
                    </div>
                    <h2 className="text-xl sm:text-2xl font-bold text-gray-900 mb-2">{t('booking.confirmed')}</h2>
                    <p className="text-sm sm:text-base text-gray-600 mb-3">{t('ticket.booked.successfully')}</p>
                    <p className="font-semibold text-sm">{t('booking.reference')}</p>
                    <p className="text-xl sm:text-2xl font-bold text-sudan-green-600">{booking.bookingRef}</p>
                    <p className="text-xs text-gray-600 mt-1">
                      {booking.boardingStationName} → {booking.alightingStationName}
                    </p>
                  </div>

                  <div className="space-y-4">
                    {(booking.passengers && booking.passengers.length > 0
                      ? booking.passengers
                      : [{ passenger: booking.passenger, seatNumber: booking.seatNumber, coachClass: booking.coachClass, price: booking.total, ticket: booking.ticket }]
                    ).map((pd, i) => (
                      <div key={i} className="bg-gray-50 rounded-lg p-4 sm:p-5 border border-gray-200">
                        <div className="flex flex-col sm:flex-row gap-4 items-center">
                          {pd.ticket?.qrPayload && (
                            <div className="bg-white border border-gray-200 rounded p-2 flex-shrink-0">
                              <QRCode value={pd.ticket.qrPayload} size={90} bgColor="#ffffff" fgColor="#064e2a" level="M" />
                            </div>
                          )}
                          <div className="flex-1 text-center sm:text-start">
                            <p className="text-sm text-gray-500 mb-1">{t('passenger')} {i + 1}</p>
                            <p className="font-semibold text-gray-900">
                              {pd.passenger?.fullNameAr || pd.passenger?.fullNameEn}
                            </p>
                            <p className="text-xs text-gray-500">{pd.passenger?.idNumber}</p>
                            <p className="text-sm mt-2">
                              {t('seat')} <span className="font-medium">{pd.seatNumber}</span>
                              <span className="text-gray-400"> · </span>
                              {pd.coachClass}
                            </p>
                            {pd.ticket?.ticketNumber && (
                              <p className="text-[11px] text-gray-500 mt-1">{pd.ticket.ticketNumber}</p>
                            )}
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>

                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 sm:gap-4 mt-6">
                    <button onClick={() => navigate('/dashboard')} className="bg-sudan-green-600 text-white py-3 rounded-lg font-medium hover:bg-sudan-green-700 text-sm sm:text-base">
                      {t('view.my.trips')}
                    </button>
                    <button onClick={() => navigate('/')} className="border border-gray-300 py-3 rounded-lg font-medium hover:bg-gray-50 text-sm sm:text-base">
                      {t('book.another.trip')}
                    </button>
                  </div>
                </div>
              )}
            </div>
          </div>

          {/* Summary */}
          <div className="lg:col-span-1">
            <div className="bg-white rounded-lg shadow-md p-4 sm:p-6 lg:sticky lg:top-24">
              <h3 className="text-base sm:text-lg font-semibold mb-4">{t('booking.summary')}</h3>
              <div className="space-y-3 sm:space-y-4 mb-4 sm:mb-6">
                <div className="flex items-center gap-3">
                  <TrainIcon className="h-5 w-5 text-sudan-green-600" />
                  <div>
                    <p className="font-medium text-sm sm:text-base">{trip.trainName}</p>
                    <p className="text-sm text-gray-600">{t(state.coachClass)}</p>
                  </div>
                </div>
                <div className="flex items-center gap-3">
                  <MapPin className="h-5 w-5 text-gray-400" />
                  <div>
                    <p className="text-sm text-gray-600">{t('route')}</p>
                    <p className="font-medium">{state.originName} → {state.destinationName}</p>
                  </div>
                </div>
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                  <div>
                    <p className="text-sm text-gray-600">{t('departure')}</p>
                    <p className="text-xs text-sudan-green-700 font-medium">{state.originName}</p>
                    <p className="text-sm text-gray-500">{formatDateSafe(trip.departureISO)}</p>
                    <p className="font-medium">{formatTimeSafe(trip.departureISO)}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-600">{t('arrival')}</p>
                    <p className="text-xs text-sudan-green-700 font-medium">{state.destinationName}</p>
                    <p className="text-sm text-gray-500">{formatDateSafe(trip.arrivalISO)}</p>
                    <p className="font-medium">{formatTimeSafe(trip.arrivalISO)}</p>
                  </div>
                </div>
                <div>
                  <p className="text-sm text-gray-600">{t('passengers')}</p>
                  <p className="font-medium">{passengerCount}</p>
                </div>
                {selectedSeats.length > 0 && (
                  <div>
                    <p className="text-sm text-gray-600">{t('seats')}</p>
                    <p className="font-medium">{selectedSeats.map((s) => s.seatNumber).join(' · ')}</p>
                  </div>
                )}
              </div>
              <div className="border-t pt-3 sm:pt-4">
                {previewFare && (previewFare.tripId || (previewFare.originStationId && previewFare.destinationStationId)) && (
                  <div className="mb-2 inline-flex items-center gap-1 px-2 py-0.5 rounded-full bg-sudan-gold-100 text-sudan-gold-800 text-[11px]">
                    {previewFare.tripId ? t('fare.scope.trip') : t('fare.scope.segment')}
                  </div>
                )}
                <div className="flex justify-between items-center mb-1">
                  <span className="text-sm text-gray-600">
                    {t('ticket.price')} × {passengerCount}
                  </span>
                  <span className="font-medium text-sm">{Math.round(unitPrice)} {t('sdg')}</span>
                </div>
                {previewBreakdown && previewBreakdown.discountAmount > 0 && (
                  <div className="flex justify-between items-center mb-1 text-sudan-green-700">
                    <span className="text-sm">− {t('discount')} ({Math.round(previewBreakdown.discountPercent)}٪)</span>
                    <span className="font-medium text-sm">−{Math.round(previewBreakdown.discountAmount * passengerCount)} {t('sdg')}</span>
                  </div>
                )}
                <div className="flex justify-between items-center text-base sm:text-lg font-bold border-t pt-2 mt-2">
                  <span>{t('total')}</span>
                  <span>{Math.round(total)} {t('sdg')}</span>
                </div>
                {currentStep >= 3 && (
                  <div className="mt-4 p-3 bg-green-50 rounded-lg">
                    <p className="text-sm text-green-800 font-medium">🔒 {t('secure.payment.guaranteed')}</p>
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
