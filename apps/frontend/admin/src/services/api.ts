// API Service Layer for Sudan Train Admin
// Handles all API communication with authentication

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:8080';
const API_PREFIX = '/Api/V1';

// Generic API Response type matching backend Response<T>
export interface ApiResponse<T> {
  statusCode: number;
  succeeded: boolean;
  message: string | null;
  data: T;
  errors: string[] | null;
  meta: any;
}

// Base fetch wrapper with authentication
async function fetchWithAuth<T>(
  endpoint: string,
  options: RequestInit = {}
): Promise<T> {
  const token = localStorage.getItem('admin_token');
  
  const headers: HeadersInit = {
    'Content-Type': 'application/json',
    ...options.headers,
  };

  if (token) {
    // 
    (headers as Record<string, string>)['Authorization'] = `Bearer ${token}`;
  }

  const response = await fetch(`${API_BASE_URL}${API_PREFIX}${endpoint}`, {
    ...options,
    headers,
  });

  if (response.status === 401) {
    // Unauthorized - clear auth and redirect to login
    localStorage.removeItem('admin_token');
    localStorage.removeItem('admin_user');
    window.location.href = '/login';
    throw new Error('Unauthorized - Please login again');
  }

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    throw new Error(
      errorData.message || 
      errorData.errors?.join(', ') || 
      `Request failed with status ${response.status}`
    );
  }

  const result: ApiResponse<T> = await response.json();
  
  if (!result.succeeded) {
    throw new Error(result.message || result.errors?.join(', ') || 'Request failed');
  }

  return result.data;
}

// Generic CRUD operations
export const api = {
  get: <T>(endpoint: string) => fetchWithAuth<T>(endpoint, { method: 'GET' }),
  
  post: <T>(endpoint: string, data: any) =>
    fetchWithAuth<T>(endpoint, {
      method: 'POST',
      body: JSON.stringify(data),
    }),
  
  put: <T>(endpoint: string, data: any) =>
    fetchWithAuth<T>(endpoint, {
      method: 'PUT',
      body: JSON.stringify(data),
    }),
  
  delete: <T>(endpoint: string) =>
    fetchWithAuth<T>(endpoint, { method: 'DELETE' }),
};

// Geography API - Cities
import { City, CityFormData, PlaceSearchResult, CityValidationResult } from '../types/geography';

export const citiesApi = {
  getAll: () => api.get<City[]>('/Infrastructure/Cities'),
  
  getById: (id: number) => api.get<City>(`/Infrastructure/Cities/${id}`),
  
  create: (data: CityFormData) =>
    api.post<City>('/Infrastructure/Cities', data),
  
  update: (id: number, data: Partial<CityFormData>) =>
    api.put<City>(`/Infrastructure/Cities/${id}`, data),

  delete: (id: number) => api.delete<void>(`/Infrastructure/Cities/${id}`),


  validateLocation: (lat: number, lng: number) =>
    api.post<CityValidationResult>('/Infrastructure/Cities/ValidateLocation', {
      latitude: lat,
      longitude: lng
    }),

  searchPlaces: async (query: string): Promise<any> => {
    return api.get<any>(`/Infrastructure/Cities/Search?query=${encodeURIComponent(query)}`);
  },

  getBoundary: (id: number) => api.get<any>(`/Infrastructure/Cities/${id}/Boundary`),

  updateBoundary: (id: number, data: any) => 
    api.put<any>(`/Infrastructure/Cities/${id}/Boundary`, data),
};

// Geography API - Stations
import { Station, StationFormData, StationValidationResult } from '../types/geography';

export const stationsApi = {
  getAll: (cityId?: number) => {
    const endpoint = cityId
      ? `/Infrastructure/Stations?cityId=${cityId}`
      : '/Infrastructure/Stations';
    return api.get<Station[]>(endpoint);
  },
  
  getById: (id: number) => api.get<Station>(`/Infrastructure/Stations/${id}`),
  
  create: (data: StationFormData) => api.post<Station>('/Infrastructure/Stations', data),
  
  bulkCreate: (stations: StationFormData[]) => 
    api.post<Station[]>('/Infrastructure/Stations/Bulk', { stations }),

  update: (id: number, data: Partial<StationFormData>) =>
    api.put<Station>(`/Infrastructure/Stations/${id}`, data),

  delete: (id: number) => api.delete<void>(`/Infrastructure/Stations/${id}`),

  validateLocation: (lat: number, lng: number, cityId: number) =>
    api.post<StationValidationResult>('/Infrastructure/Stations/ValidateLocation', {
      latitude: lat,
      longitude: lng,
      cityId: cityId
    }),
};

// Spatial API
export const spatialApi = {
  validateLocation: (data: {
    latitude: number;
    longitude: number;
    parentType: string;
    parentId: number;
  }) => api.post<{ isValid: boolean; message: string }>('/Infrastructure/Spatial/ValidateLocation', data),

  reverseGeocode: (lat: number, lng: number) =>
    api.post<any>('/Infrastructure/Spatial/ReverseGeocode', { latitude: lat, longitude: lng }),

  getNearbyStations: (lat: number, lng: number, radiusKm: number) =>
    api.get<any[]>(`/Infrastructure/Spatial/NearbyStations?lat=${lat}&lng=${lng}&radiusKm=${radiusKm}`),

  calculateDistance: (lat1: number, lng1: number, lat2: number, lng2: number) =>
    api.post<{ distanceKm: number }>('/Infrastructure/Spatial/CalculateDistance', {
      lat1,
      lng1,
      lat2,
      lng2,
    }),
};

// Boundary API
export interface BoundaryData {
  boundaryPolygon?: string | null;
  boundingBoxNorth?: number | null;
  boundingBoxSouth?: number | null;
  boundingBoxEast?: number | null;
  boundingBoxWest?: number | null;
}

export const boundaryApi = {
  // Region boundaries
  getRegionBoundary: (id: number) =>
    api.get<BoundaryData>(`/Infrastructure/Areas/${id}/Boundary`),

  updateRegionBoundary: (id: number, data: BoundaryData) =>
    api.put<void>(`/Infrastructure/Areas/${id}/Boundary`, data),

  // State boundaries
  getStateBoundary: (id: number) =>
    api.get<BoundaryData>(`/Infrastructure/Governorates/${id}/Boundary`),

  updateStateBoundary: (id: number, data: BoundaryData) =>
    api.put<void>(`/Infrastructure/Governorates/${id}/Boundary`, data),

  // City boundaries
  getCityBoundary: (id: number) =>
    api.get<BoundaryData>(`/Infrastructure/Cities/${id}/Boundary`),

  updateCityBoundary: (id: number, data: BoundaryData) =>
    api.put<void>(`/Infrastructure/Cities/${id}/Boundary`, data),
};
