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

// Fare interface
export interface Fare {
  id: number;
  routeId?: number;
  originStationId?: number;
  destinationStationId?: number;
  tripId?: number;
  coachClass: string;
  basePrice: number;
  pricePerKm?: number;
  vatRate: number;
  discountPercent?: number;
  currency: string;
  finalPrice: number;
  totalWithVat: number;
  effectiveFrom: string;
  effectiveTo?: string;
}

// Fare form data
export interface FareFormData {
  routeId?: number;
  originStationId?: number;
  destinationStationId?: number;
  tripId?: number;
  coachClass: number; // Enum: 1=First, 2=Second, 3=Third
  basePrice: number;
  pricePerKm?: number;
  vatRate: number;
  discountPercent?: number;
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
