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

export const TRIP_STATUSES = ['Scheduled', 'Departed', 'Completed', 'Cancelled', 'Delayed'] as const;

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
