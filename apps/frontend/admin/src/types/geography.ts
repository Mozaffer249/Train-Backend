// TypeScript types for Geography Management
// Matching backend DTOs from Controllers/Infrastructure/Geography

export interface City {
  id: number;
  nameAr: string;
  nameEn: string;
  latitude: number;
  longitude: number;
  googlePlaceId?: string;
  formattedAddress?: string;
  boundaryPolygon?: string;
  boundingBoxNorth?: number;
  boundingBoxSouth?: number;
  boundingBoxEast?: number;
  boundingBoxWest?: number;
  createdAt?: string;
  updatedAt?: string;
}

export interface CityFormData {
  nameAr: string;
  nameEn: string;
  latitude: number;
  longitude: number;
  googlePlaceId?: string;
  formattedAddress?: string;
  boundaryPolygon?: string;
  boundingBoxNorth?: number;
  boundingBoxSouth?: number;
  boundingBoxEast?: number;
  boundingBoxWest?: number;
}

// Station interface
export interface Station {
  id: number;
  code: string;
  nameAr: string;
  nameEn: string;
  cityId: number;
  cityName?: string;
  latitude: number;
  longitude: number;
  stationType?: string;
  serviceRadiusKm?: number;
  googlePlaceId?: string;
  formattedAddress?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface StationFormData {
  code: string;
  nameAr: string;
  nameEn: string;
  cityId: number;
  latitude: number;
  longitude: number;
  stationType?: string;
  serviceRadiusKm?: number;
  googlePlaceId?: string;
  formattedAddress?: string;
}

// Boundary data interface (matching backend BoundaryDto)
export interface BoundaryData {
  boundaryPolygon?: string;
  boundingBoxNorth?: number;
  boundingBoxSouth?: number;
  boundingBoxEast?: number;
  boundingBoxWest?: number;
}

// Google Places search result
export interface PlaceSearchResult {
  placeId: string;
  description: string;
  mainText: string;
  secondaryText: string;
}

// City validation result (for duplicate detection)
export interface CityValidationResult {
  isValid: boolean;
  message: string;
  existingCity?: City;
  suggestedData?: {
    nameEn: string;
    formattedAddress: string;
    googlePlaceId?: string;
    boundaryPolygon?: string;
    boundingBoxNorth?: number;
    boundingBoxSouth?: number;
    boundingBoxEast?: number;
    boundingBoxWest?: number;
  };
  distanceKm?: number;
}

// Map entity union type
export type MapEntity = City | Station;
export type EntityType = 'city' | 'station';

// Tab types for Geography page
export type GeographyTab = 'cities' | 'stations' | 'map';
