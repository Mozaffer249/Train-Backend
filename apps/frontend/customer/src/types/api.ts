// Shared API types for the customer app.
// DTO shapes mirror the backend Response<T> envelope and Infrastructure/Auth DTOs.

export interface ApiResponse<T> {
  statusCode: number;
  succeeded: boolean;
  message: string | null;
  data: T;
  errors: string[] | null;
  meta: unknown;
}

// ---- Catalog / search DTOs (real backend) ----
export interface StationDto {
  id: number;
  code: string;
  nameEn: string;
  nameAr: string;
  cityId: number;
  cityName: string;
  latitude: number;
  longitude: number;
  stationType?: string | null;
  isActive: boolean;
}

export interface RouteStationDto {
  id: number;
  stationId: number;
  stationName: string;
  stopOrder: number;
  arrivalOffset?: number | string | null;
  departureOffset?: number | string | null;
}

export interface RouteDto {
  id: number;
  nameEn: string;
  nameAr: string;
  origin: StationDto | null;
  destination: StationDto | null;
  distanceKm?: number | null;
  isActive: boolean;
  maintenanceNote?: string | null;
  intermediateStops: RouteStationDto[];
  tripsCount: number;
}

export interface TripDto {
  id: number;
  trainId: number;
  trainNumber: string;
  trainName: string;
  routeId: number;
  routeName: string;
  originStation: string;
  destinationStation: string;
  departureTime: string; // ISO date-time
  arrivalTime: string; // ISO date-time
  status: string;
  totalSeats: number;
  availableSeats: number;
  bookedSeats: number;
}

// base − discount = total. No VAT.
export interface FareBreakdownDto {
  basePrice: number;
  discountPercent: number;
  discountAmount: number;
  total: number;
  currency: string;
}

export interface FareDto {
  id: number;
  routeId?: number | null;
  originStationId?: number | null;
  destinationStationId?: number | null;
  tripId?: number | null;
  coachClass: number | string;
  basePrice: number;
  discountPercent?: number | null;
  currency: string;
  finalPrice: number;
  breakdown?: FareBreakdownDto | null;
}

// ---- Auth DTOs (real backend) ----
export interface JwtAuthResult {
  accessToken: string;
  refreshToken: { userName: string; tokenString: string; expireAt: string } | null;
  requiresTwoFactor: boolean;
  isNewDevice: boolean;
  deviceId: string | null;
  userId: number;
  userName: string;
  email: string;
  fullName: string | null;
  roles: string[];
}

export interface RegisterPayload {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
  phoneNumber?: string;
}

// ---- Segment seat availability (real backend) ----
export interface AvailableSeatDto {
  id: number;
  tripSeatId: number;
  seatNumber: string;
  isWindow: boolean;
  isAccessible: boolean;
  isAvailable: boolean;
}

export interface CoachSeatsDto {
  id: number;
  coachNumber: string;
  class: string;
  seats: AvailableSeatDto[];
}

export interface SegmentSeatsDto {
  tripId: number;
  boardingStationId: number;
  alightingStationId: number;
  boardingStationName: string;
  alightingStationName: string;
  totalSeats: number;
  availableCount: number;
  coaches: CoachSeatsDto[];
}

// ---- Booking types (real backend) ----
export interface PassengerInput {
  fullNameEn: string;
  fullNameAr?: string;
  idNumber: string;
  birthDate?: string;
  gender?: 'male' | 'female' | '';
  nationality?: string;
  phone?: string;
  email?: string;
}

// Backend BookingStatus is one of Pending|Confirmed|Cancelled|Completed.
// We keep it as a string so we can compare freely in the UI.
export type BookingStatus = 'Pending' | 'Confirmed' | 'Cancelled' | 'Completed';

export interface BookingPassengerInfoDto {
  fullNameEn: string;
  fullNameAr?: string | null;
  idNumber: string;
  phone?: string | null;
  email?: string | null;
  gender?: string | null;
  nationality?: string | null;
}

export interface TicketInfoDto {
  ticketNumber: string;
  qrPayload?: string | null;
  status: string;
}

// Per-passenger detail on a booking — one entry per booked seat.
export interface BookingPassengerDetailDto {
  passenger: BookingPassengerInfoDto;
  seatNumber: string;
  coachClass: string;
  price: number;
  ticket?: TicketInfoDto | null;
}

export interface BookingDto {
  id: number;
  bookingRef: string;
  tripId: number;
  trainName: string;
  routeName: string;

  boardingStationId: number;
  alightingStationId: number;
  boardingStationName: string;
  alightingStationName: string;
  departureTime: string; // ISO
  arrivalTime: string;   // ISO

  // Convenience: primary (first) passenger's seat + class. For multi-seat
  // bookings prefer the `passengers` list as the source of truth.
  coachClass: string;
  seatNumber: string;
  passenger: BookingPassengerInfoDto;
  ticket?: TicketInfoDto | null;

  // All passengers on this booking (1+ entries).
  passengers: BookingPassengerDetailDto[];

  basePrice: number;
  total: number; // sum across all passengers
  currency: string;

  // Per-seat price walk (same for every seat in the booking).
  breakdown?: FareBreakdownDto | null;

  status: BookingStatus;
  createdAt: string;
}

// Multi-seat booking payload. Each entry pairs a seat with the passenger riding it.
export interface CreateBookingPassengerPayload {
  seatId: number;
  coachClass: number; // 1=First, 2=Second, 3=Third
  passenger: PassengerInput;
}

export interface CreateBookingPayload {
  tripId: number;
  boardingStationId: number;
  alightingStationId: number;
  paymentMethod: number; // 0=Cash, 1=CreditCard, 2=DebitCard, 3=BankTransfer, 4=MobilePayment
  cardLast4?: string;
  passengers: CreateBookingPassengerPayload[];
}
