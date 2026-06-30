// API Service Layer for Sudan Train Admin
// Handles all API communication with authentication

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:8081';
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
    // Handle both camelCase and PascalCase from backend
    const message = errorData.message || errorData.Message;
    const errors = errorData.errors || errorData.Errors;
    throw new Error(
      message || 
      errors?.join(', ') || 
      `Request failed with status ${response.status}`
    );
  }

  const result: ApiResponse<T> = await response.json();
  
  if (!result.succeeded) {
    // Handle both camelCase and PascalCase from backend
    const message = result.message || (result as any).Message;
    const errors = result.errors || (result as any).Errors;
    throw new Error(message || errors?.join(', ') || 'Request failed');
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
import { City, CityFormData, CityValidationResult } from '../types/geography';

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
  getAll: (params?: {
    cityId?: number;
    searchTerm?: string;
    isActive?: boolean;
    stationType?: string;
    pageNumber?: number;
    pageSize?: number;
  }) => {
    const queryParams = new URLSearchParams();
    if (params?.cityId) queryParams.append('cityId', params.cityId.toString());
    if (params?.searchTerm) queryParams.append('searchTerm', params.searchTerm);
    if (params?.isActive !== undefined) queryParams.append('isActive', params.isActive.toString());
    if (params?.stationType) queryParams.append('stationType', params.stationType);
    if (params?.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString());
    if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString());
    
    const endpoint = queryParams.toString() 
      ? `/Infrastructure/Stations?${queryParams}`
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

// Routes API
import { Route, RouteFormData, RouteStationFormData } from '../types/infrastructure';

// Helper function to build query params
function buildQueryParams(params?: Record<string, any>): string {
  if (!params) return '';
  const queryParams = new URLSearchParams();
  Object.keys(params).forEach(key => {
    if (params[key] !== undefined && params[key] !== null) {
      queryParams.append(key, params[key].toString());
    }
  });
  return queryParams.toString();
}

export const routesApi = {
  getAll: (params?: {
    originStationId?: number;
    destinationStationId?: number;
    isActive?: boolean;
    pageNumber?: number;
    pageSize?: number;
  }) => {
    const queryString = buildQueryParams(params);
    const endpoint = queryString ? `/Infrastructure/Routes?${queryString}` : '/Infrastructure/Routes';
    return api.get<Route[]>(endpoint);
  },
  
  getById: (id: number) => api.get<Route>(`/Infrastructure/Routes/${id}`),
  
  create: (data: RouteFormData) => api.post<Route>('/Infrastructure/Routes', data),
  
  update: (id: number, data: Partial<RouteFormData>) =>
    api.put<Route>(`/Infrastructure/Routes/${id}`, data),
  
  delete: (id: number) => api.delete<void>(`/Infrastructure/Routes/${id}`),
  
  // Route stations management
  addStation: (routeId: number, data: RouteStationFormData) => 
    api.post(`/Infrastructure/Routes/${routeId}/Stations`, data),
  
  updateStation: (routeId: number, stationId: number, data: Partial<RouteStationFormData>) =>
    api.put(`/Infrastructure/Routes/${routeId}/Stations/${stationId}`, data),
  
  removeStation: (routeId: number, stationId: number) =>
    api.delete(`/Infrastructure/Routes/${routeId}/Stations/${stationId}`),
};

// Fares API
import { Fare, FareFormData } from '../types/infrastructure';

export const faresApi = {
  getAll: (params?: {
    routeId?: number;
    coachClass?: number;
    pageNumber?: number;
    pageSize?: number;
  }) => {
    const queryString = buildQueryParams(params);
    const endpoint = queryString ? `/Infrastructure/Fares?${queryString}` : '/Infrastructure/Fares';
    return api.get<Fare[]>(endpoint);
  },

  create: (data: FareFormData) => api.post<Fare>('/Infrastructure/Fares', data),

  // PATCH-style update; only send the fields the admin actually changed.
  update: (id: number, data: Partial<FareFormData>) =>
    api.put<Fare>(`/Infrastructure/Fares/${id}`, data),
};

// Trains API
import { Train, TrainFormData, BulkCoachesFormData, Trip, TripFormData, TripUpdateData, CoachUpdateData } from '../types/infrastructure';

// Coach row shape returned by /Infrastructure/Trains/{trainId}/Coaches and
// the new /Coaches/{id} endpoint. Capacity is read-only in the admin UI once
// seats exist; class change re-classifies but doesn't regenerate the grid.
export interface AdminCoach {
  id: number;
  trainId: number;
  coachNumber: string;
  class: string;
  capacity: number;
  sequence: number;
  seatsCount: number;
}

export const trainsApi = {
  getAll: () => api.get<Train[]>('/Infrastructure/Trains'),

  getById: (id: number) => api.get<Train>(`/Infrastructure/Trains/${id}`),

  create: (data: TrainFormData) => api.post<Train>('/Infrastructure/Trains', data),

  update: (id: number, data: TrainFormData) =>
    api.put<Train>(`/Infrastructure/Trains/${id}`, { id, ...data }),

  delete: (id: number) => api.delete<void>(`/Infrastructure/Trains/${id}`),

  getCoaches: (trainId: number) => api.get<AdminCoach[]>(`/Infrastructure/Trains/${trainId}/Coaches`),

  getCoach: (coachId: number) => api.get<AdminCoach>(`/Infrastructure/Trains/Coaches/${coachId}`),

  updateCoach: (coachId: number, data: CoachUpdateData) =>
    api.put<AdminCoach>(`/Infrastructure/Trains/Coaches/${coachId}`, data),

  bulkCreateCoaches: (trainId: number, data: BulkCoachesFormData) =>
    api.post(`/Infrastructure/Trains/${trainId}/Coaches/Bulk`, { trainId, ...data }),
};

// Per-coach seat layout. Returned by GET /Infrastructure/Coaches/{coachId}/Seats.
export interface AdminSeat {
  id: number;
  coachId: number;
  seatNumber: string;
  isWindow: boolean;
  isAccessible: boolean;
}

export const coachesApi = {
  getSeats: (coachId: number) =>
    api.get<AdminSeat[]>(`/Infrastructure/Coaches/${coachId}/Seats`),
};

// Trips API
export const tripsApi = {
  getAll: (params?: { date?: string; routeId?: number; status?: string; upcomingOnly?: boolean }) => {
    const queryString = buildQueryParams(params);
    const endpoint = queryString ? `/Infrastructure/Trips?${queryString}` : '/Infrastructure/Trips';
    return api.get<Trip[]>(endpoint);
  },

  getById: (id: number) => api.get<Trip>(`/Infrastructure/Trips/${id}`),

  create: (data: TripFormData) => api.post<Trip>('/Infrastructure/Trips', data),

  update: (id: number, data: TripUpdateData) =>
    api.put<Trip>(`/Infrastructure/Trips/${id}`, { id, ...data }),

  cancel: (id: number) => api.put<Trip>(`/Infrastructure/Trips/${id}/Cancel`, {}),

  // Per-segment seat availability — used by the counter sale flow.
  getSegmentSeats: (tripId: number, boardingStationId: number, alightingStationId: number) => {
    const qs = buildQueryParams({ boardingStationId, alightingStationId });
    return api.get<import('../types/infrastructure').SegmentSeatsDto>(
      `/Infrastructure/Trips/${tripId}/Seats?${qs}`,
    );
  },
};

// Bookings API (per-segment)
import {
  Booking,
  AdminUser,
  UserFormData,
  MeInfo,
  CustomerSummary,
  TripManifest,
  ScanResult,
  Refund,
  Notification,
  PaymentsReport,
  CounterBookingPayload,
} from '../types/infrastructure';

export const bookingsApi = {
  getAll: (params?: { status?: string; userId?: number; pageNumber?: number; pageSize?: number }) => {
    const queryString = buildQueryParams(params);
    const endpoint = queryString ? `/Bookings?${queryString}` : '/Bookings';
    return api.get<Booking[]>(endpoint);
  },

  getById: (id: number) => api.get<Booking>(`/Bookings/${id}`),

  cancel: (id: number, reason?: string) =>
    api.post<string>(`/Bookings/${id}/Cancel`, { bookingId: id, reason }),

  createCounter: (payload: CounterBookingPayload) =>
    api.post<Booking>('/Bookings/Counter', payload),
};

// ----- Identity (current user) -----

export const authApi = {
  me: () => api.get<MeInfo>('/Account/Me'),
};

// ----- Users management (Admin only for writes, AnyStaff for reads) -----

export const usersApi = {
  getAll: (params?: {
    pageNumber?: number;
    pageSize?: number;
    search?: string;
    role?: string;
    isActive?: boolean;
    excludePrivileged?: boolean;
  }) => {
    const qs = buildQueryParams(params);
    const endpoint = qs ? `/Admin/Users?${qs}` : '/Admin/Users?excludePrivileged=true';
    return api.get<AdminUser[]>(endpoint);
  },

  getById: (id: number) => api.get<AdminUser>(`/Admin/Users/${id}`),

  create: (data: UserFormData) => api.post<AdminUser>('/Admin/Users', data),

  update: (id: number, data: Partial<UserFormData>) =>
    api.put<AdminUser>(`/Admin/Users/${id}`, data),

  setActive: (id: number, isActive: boolean) =>
    api.put<string>(`/Admin/Users/${id}/Active`, { id, isActive }),

  assignRoles: (id: number, roles: string[]) =>
    api.put<string[]>(`/Admin/Users/${id}/Roles`, { id, roles }),

  assignStations: (id: number, stationIds: number[]) =>
    api.put<string>(`/Admin/Users/${id}/Stations`, { id, stationIds }),

  lookup: (query: string) =>
    api.get<CustomerSummary[]>(`/Admin/Users/Lookup?query=${encodeURIComponent(query)}`),
};

// ----- Admin account management (SuperAdmin only) -----

export const adminsApi = {
  getAll: (params?: {
    pageNumber?: number;
    pageSize?: number;
    search?: string;
    isActive?: boolean;
  }) => {
    const qs = buildQueryParams(params);
    const endpoint = qs ? `/Admin/Admins?${qs}` : '/Admin/Admins';
    return api.get<AdminUser[]>(endpoint);
  },

  create: (data: UserFormData) => api.post<AdminUser>('/Admin/Admins', data),

  update: (id: number, data: Partial<UserFormData>) =>
    api.put<AdminUser>(`/Admin/Admins/${id}`, data),

  setActive: (id: number, isActive: boolean) =>
    api.put<string>(`/Admin/Admins/${id}/Active`, { id, isActive }),

  assignRoles: (id: number, roles: string[]) =>
    api.put<string[]>(`/Admin/Admins/${id}/Roles`, { id, roles }),
};

// ----- Boarding (manifest, board, scan, no-show) -----

export const boardingApi = {
  getManifest: (tripId: number, boardingStationId?: number) => {
    const qs = boardingStationId ? `?boardingStationId=${boardingStationId}` : '';
    return api.get<TripManifest>(`/Trips/${tripId}/Manifest${qs}`);
  },

  boardTicket: (ticketId: number) =>
    api.post<string>(`/Tickets/${ticketId}/Board`, {}),

  scan: (qrPayload: string) =>
    api.post<ScanResult>('/Tickets/Scan', { qrPayload }),

  markNoShow: (ticketId: number) =>
    api.post<string>(`/Tickets/${ticketId}/NoShow`, {}),

  departTrip: (tripId: number) =>
    api.post<string>(`/Infrastructure/Trips/${tripId}/Depart`, {}),

  arriveTrip: (tripId: number) =>
    api.post<string>(`/Infrastructure/Trips/${tripId}/Arrive`, {}),
};

// ----- Refunds (Admin only) -----

export const refundsApi = {
  getAll: (status?: string) => {
    const qs = status ? `?status=${encodeURIComponent(status)}` : '';
    return api.get<Refund[]>(`/Refunds${qs}`);
  },

  approve: (id: number, note?: string) =>
    api.post<string>(`/Refunds/${id}/Approve`, { id, note }),

  reject: (id: number, reason?: string) =>
    api.post<string>(`/Refunds/${id}/Reject`, { id, reason }),
};

// ----- Payments report (Admin only) -----

export const paymentsApi = {
  getReport: (params?: {
    fromDate?: string;
    toDate?: string;
    method?: string;
    status?: string;
    pageNumber?: number;
    pageSize?: number;
  }) => {
    const queryString = buildQueryParams(params);
    return api.get<PaymentsReport>(`/Payments${queryString ? `?${queryString}` : ''}`);
  },
};

// ----- Notifications (current user) -----

export const notificationsApi = {
  mine: (unreadOnly?: boolean) => {
    const qs = unreadOnly ? '?unreadOnly=true' : '';
    return api.get<Notification[]>(`/Notifications/Mine${qs}`);
  },

  markRead: (id: number) => api.post<string>(`/Notifications/${id}/Read`, {}),
};
