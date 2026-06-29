// TypeScript types for Infrastructure Management
// Routes, Fares, and related DTOs

// Simplified station DTO for route display
export interface StationDto {
  id: number;
  code: string;
  nameEn: string;
  nameAr: string;
  cityId: number;
  cityName: string;
  latitude: number;
  longitude: number;
}

// Route Station DTO
export interface RouteStationDto {
  id: number;
  stationId: number;
  stationName: string;
  stopOrder: number;
  arrivalOffset: string; // TimeSpan format "HH:MM:SS"
  departureOffset: string; // TimeSpan format "HH:MM:SS"
}

// Route interface
export interface Route {
  id: number;
  nameEn: string;
  nameAr: string;
  origin: StationDto;
  destination: StationDto;
  distanceKm?: number;
  isActive: boolean;
  maintenanceNote?: string;
  intermediateStops: RouteStationDto[];
  tripsCount: number;
}

// Route form data
export interface RouteFormData {
  nameEn?: string;
  nameAr?: string;
  originStationId: number;
  destinationStationId: number;
  distanceKm?: number;
  isActive?: boolean;
  maintenanceNote?: string;
}

// Route station form data
export interface RouteStationFormData {
  stationId: number;
  stopOrder: number;
  arrivalMinutesFromOrigin: number;
  departureMinutesFromOrigin: number;
}

// Fare interface (MVP). Polymorphic scope (route/segment/trip) + flat pricing:
// basePrice − discount = total. No VAT.
export interface Fare {
  id: number;
  routeId?: number;
  originStationId?: number;
  destinationStationId?: number;
  tripId?: number;
  coachClass: string;
  basePrice: number;
  discountPercent?: number;
  currency: string;
  finalPrice: number;
  effectiveFrom: string;
  effectiveTo?: string;
}

// Form payload used by both Create (POST) and Update (PUT). Scope columns are
// only meaningful on create — admin UI greys them out in edit mode.
export interface FareFormData {
  routeId?: number;
  originStationId?: number;
  destinationStationId?: number;
  tripId?: number;
  coachClass: number; // 1=First, 2=Second, 3=Third
  basePrice: number;
  discountPercent?: number;
  effectiveFrom?: string;
  effectiveTo?: string;
}

// Coach update payload — capacity is intentionally not editable here.
export interface CoachUpdateData {
  coachNumber?: string;
  class?: number; // CoachClass enum value
  sequence?: number;
}

// Pagination params
export interface PaginationParams {
  pageNumber: number;
  pageSize: number;
}

// Coach class enum
export enum CoachClass {
  First = 1,
  Second = 2,
  Third = 3,
}

// Coach class labels
export const CoachClassLabels: Record<number, string> = {
  1: 'First Class',
  2: 'Second Class',
  3: 'Third Class',
};

// Train interface (matches backend TrainDto). A train has no single class —
// each coach carries its own class; mixed-class trains are the norm.
export interface Train {
  id: number;
  trainNumber: string;
  nameEn: string;
  nameAr: string;
  coachesCount: number;
  totalCapacity: number;
  createdAt: string;
}

export interface TrainFormData {
  trainNumber: string;
  nameEn: string;
  nameAr: string;
}

export interface BulkCoachesFormData {
  numberOfCoaches: number;
  class: number; // CoachClass enum
  capacityPerCoach: number;
  autoGenerateSeats: boolean;
}

// Trip interface (matches backend TripDto)
export interface Trip {
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

export interface TripFormData {
  trainId: number;
  routeId: number;
  departureTime: string;
  arrivalTime: string;
}

export interface TripUpdateData {
  departureTime: string;
  arrivalTime: string;
  status: string;
}

export const TRIP_STATUSES = ['Scheduled', 'Departed', 'Arrived', 'Cancelled', 'Delayed'] as const;

// ---- Bookings (per-segment) ----
export interface BookingPassengerInfo {
  fullNameEn: string;
  fullNameAr?: string | null;
  idNumber: string;
  phone?: string | null;
  email?: string | null;
  gender?: string | null;
  nationality?: string | null;
}

export interface BookingTicketInfo {
  ticketNumber: string;
  qrPayload?: string | null;
  status: string;
}

export interface Booking {
  id: number;
  bookingRef: string;
  tripId: number;
  trainName: string;
  routeName: string;
  boardingStationId: number;
  alightingStationId: number;
  boardingStationName: string;
  alightingStationName: string;
  departureTime: string;
  arrivalTime: string;
  coachClass: string;
  seatNumber: string;
  passenger: BookingPassengerInfo;
  basePrice: number;
  vatAmount: number;
  total: number;
  currency: string;
  status: string;
  createdAt: string;
  ticket?: BookingTicketInfo | null;
}

export const BOOKING_STATUSES = ['Pending', 'Confirmed', 'Cancelled', 'Completed'] as const;

// ----- Identity / users -----

export interface AdminUser {
  id: number;
  userName: string;
  firstName?: string | null;
  lastName?: string | null;
  email?: string | null;
  phoneNumber?: string | null;
  isActive: boolean;
  createdAt?: string | null;
  roles: string[];
  stationIds: number[];
}

export interface UserFormData {
  firstName?: string;
  lastName?: string;
  userName: string;
  email: string;
  password?: string;
  phoneNumber?: string;
  roles?: string[];
  stationIds?: number[];
}

export interface MeInfo {
  userId: number;
  userName?: string | null;
  email?: string | null;
  fullName?: string | null;
  roles: string[];
  assignedStationIds: number[];
}

export interface CustomerSummary {
  userId: number;
  fullName?: string | null;
  email?: string | null;
  phoneNumber?: string | null;
  userName?: string | null;
  idNumber?: string | null;
}

// ----- Boarding / manifest -----

export interface ManifestRow {
  ticketId: number;
  ticketNumber?: string | null;
  bookingId: number;
  bookingReference?: string | null;
  passengerNameEn?: string | null;
  passengerNameAr?: string | null;
  idNumber?: string | null;
  seatNumber?: string | null;
  coachNumber?: string | null;
  coachClass?: string | null;
  boardingStationId: number;
  boardingStationEn?: string | null;
  boardingStationAr?: string | null;
  alightingStationId: number;
  alightingStationEn?: string | null;
  alightingStationAr?: string | null;
  status: string;
  boardedAt?: string | null;
  boardedByUserId?: number | null;
}

export interface TripManifest {
  tripId: number;
  trainNumber?: string | null;
  routeNameEn?: string | null;
  routeNameAr?: string | null;
  originStationEn?: string | null;
  originStationAr?: string | null;
  destinationStationEn?: string | null;
  destinationStationAr?: string | null;
  departureTime: string;
  arrivalTime: string;
  status: string;
  totalTickets: number;
  boardedCount: number;
  issuedCount: number;
  noShowCount: number;
  cancelledCount: number;
  rows: ManifestRow[];
}

export interface ScanResult {
  ticketId: number;
  ticketNumber?: string | null;
  status: string;
  passengerName?: string | null;
  seatNumber?: string | null;
  tripId: number;
}

// ----- Refunds + notifications -----

export interface Refund {
  id: number;
  refundNumber: string;
  bookingId: number;
  bookingReference?: string | null;
  userId?: number | null;
  userFullName?: string | null;
  amount: number;
  currency: string;
  status: string;
  method: string;
  reason?: string | null;
  processedAt?: string | null;
  createdAt: string;
}

export interface Notification {
  id: number;
  bookingId?: number | null;
  bookingReference?: string | null;
  type: string;
  subject: string;
  message: string;
  isRead: boolean;
  readAt?: string | null;
  createdAt: string;
}

// ----- Payments report -----

export const PAYMENT_METHODS = ['Cash', 'CreditCard', 'DebitCard', 'BankTransfer', 'MobilePayment'] as const;
export const PAYMENT_STATUSES = ['Pending', 'Completed', 'Failed', 'Refunded'] as const;

export interface PaymentReportItem {
  id: number;
  bookingId: number;
  bookingRef: string;
  customerName?: string | null;
  method: string;
  status: string;
  amount: number;
  currency: string;
  cardBrand?: string | null;
  cardLast4?: string | null;
  reference?: string | null;
  createdAt: string;
}

export interface PaymentsReport {
  items: PaymentReportItem[];
  summary: {
    totalCollected: number;
    count: number;
    byStatus: { status: string; count: number; amount: number }[];
    byMethod: { method: string; count: number; amount: number }[];
  };
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

// ----- Counter booking payload -----

export interface CounterPassengerInput {
  fullNameEn: string;
  fullNameAr?: string;
  idNumber: string;
  phone?: string;
  email?: string;
  gender?: string;
  nationality?: string;
  birthDate?: string;
}

export interface CounterSeatInput {
  seatId: number;
  // Backend `CoachClass` is a numeric enum: 1=First, 2=Second, 3=Third.
  // The wire format is the int; convert from the string class name reported
  // by SegmentSeatsDto via `coachClassNameToId()` at submit time.
  coachClass: number;
  passenger: CounterPassengerInput;
}

// Maps the customer-facing class name string from CoachSeatsDto.class to
// the backend's numeric CoachClass enum value.
export function coachClassNameToId(name: string | undefined | null): number {
  switch ((name ?? '').toLowerCase()) {
    case 'first': return 1;
    case 'third': return 3;
    case 'second':
    default: return 2;
  }
}

export interface CounterBookingPayload {
  customerUserId?: number | null;
  tripId: number;
  boardingStationId: number;
  alightingStationId: number;
  // Backend `PaymentMethod` is a numeric enum:
  //   0=Cash, 1=CreditCard, 2=DebitCard, 3=BankTransfer, 4=MobilePayment.
  // Send the number; the JSON deserializer doesn't accept the enum name.
  paymentMethod?: number;
  cardLast4?: string;
  passengers: CounterSeatInput[];
}

// ----- Role constants (mirror Roles.cs) -----
export const ROLES = {
  SuperAdmin: 'SuperAdmin',
  Admin: 'Admin',
  Staff: 'Staff',
  StaffCounter: 'StaffCounter',
  StaffBoarding: 'StaffBoarding',
  Customer: 'Customer',
  User: 'User',
} as const;
export type Role = (typeof ROLES)[keyof typeof ROLES];

export const TICKET_STATUSES = ['Issued', 'Boarded', 'NoShow', 'Cancelled'] as const;
export const REFUND_STATUSES = ['Pending', 'Approved', 'Rejected', 'Completed'] as const;

// ----- Per-segment seat availability (mirror of customer-app SegmentSeatsDto) -----

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
