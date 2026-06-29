// API service layer for the Sudan Train customer app.
// Mirrors the admin app's client: Response<T> envelope, Bearer token, 401 handling.

import type {
  ApiResponse,
  StationDto,
  RouteDto,
  TripDto,
  FareDto,
  JwtAuthResult,
  ProfileResponse,
  RegisterPayload,
  SegmentSeatsDto,
} from '../types/api';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:8081';
const API_PREFIX = '/Api/V1';

export const TOKEN_KEY = 'customer_token';
export const USER_KEY = 'customer_user';

async function fetchWithAuth<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
  const token = localStorage.getItem(TOKEN_KEY);

  const headers: HeadersInit = {
    'Content-Type': 'application/json',
    ...options.headers,
  };
  if (token) {
    (headers as Record<string, string>)['Authorization'] = `Bearer ${token}`;
  }

  const response = await fetch(`${API_BASE_URL}${API_PREFIX}${endpoint}`, { ...options, headers });

  if (response.status === 401) {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    if (window.location.pathname !== '/login') {
      window.location.href = '/login';
    }
    throw new Error('Unauthorized - please login again');
  }

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    const message = errorData.message || errorData.Message;
    const errors = errorData.errors || errorData.Errors;
    throw new Error(message || errors?.join(', ') || `Request failed with status ${response.status}`);
  }

  const result: ApiResponse<T> = await response.json();
  if (!result.succeeded) {
    const message = result.message || (result as { Message?: string }).Message;
    const errors = result.errors || (result as { Errors?: string[] }).Errors;
    throw new Error(message || errors?.join(', ') || 'Request failed');
  }
  return result.data;
}

export const api = {
  get: <T>(endpoint: string) => fetchWithAuth<T>(endpoint, { method: 'GET' }),
  post: <T>(endpoint: string, data: unknown) =>
    fetchWithAuth<T>(endpoint, { method: 'POST', body: JSON.stringify(data) }),
  put: <T>(endpoint: string, data: unknown) =>
    fetchWithAuth<T>(endpoint, { method: 'PUT', body: JSON.stringify(data) }),
  delete: <T>(endpoint: string) => fetchWithAuth<T>(endpoint, { method: 'DELETE' }),
};

function buildQuery(params?: Record<string, string | number | boolean | undefined>): string {
  if (!params) return '';
  const qp = new URLSearchParams();
  Object.entries(params).forEach(([k, v]) => {
    if (v !== undefined && v !== null && v !== '') qp.append(k, String(v));
  });
  return qp.toString();
}

// ---- Auth API (real backend) ----
export interface RegisterResult {
  message?: string;
  userId?: number;
  email?: string;
  // Set by the backend when the email already belonged to an unconfirmed account
  // and a fresh OTP was sent so the user can finish confirmation.
  resumeConfirmation?: boolean;
}

export const authApi = {
  register: (data: RegisterPayload) =>
    api.post<RegisterResult>('/Authentication/Register', data),

  confirmEmail: (userId: number, code: string) =>
    api.post<string>('/Authentication/ConfirmEmail', { userId, code }),

  login: (userNameOrEmail: string, password: string) =>
    api.post<JwtAuthResult>('/Authentication/Login', { userNameOrEmail, password }),

  getProfile: () => api.get<ProfileResponse>('/Authentication/Profile'),

  sendResetCode: (email: string) =>
    api.post<string>('/Authentication/SendResetPasswordCode', { email }),

  resetPassword: (data: { email: string; resetCode: string; newPassword: string; confirmPassword: string }) =>
    api.post<string>('/Authentication/ResetPassword', data),

  logout: () => api.post<string>('/Authentication/Logout', {}),
};

// ---- Catalog / search API (real backend, public) ----
export const catalogApi = {
  getStations: (params?: { cityId?: number; searchTerm?: string; isActive?: boolean; pageNumber?: number; pageSize?: number }) => {
    const qs = buildQuery(params);
    return api.get<StationDto[]>(qs ? `/Infrastructure/Stations?${qs}` : '/Infrastructure/Stations');
  },

  getRoutes: (params?: { originStationId?: number; destinationStationId?: number; isActive?: boolean; pageNumber?: number; pageSize?: number }) => {
    const qs = buildQuery(params);
    return api.get<RouteDto[]>(qs ? `/Infrastructure/Routes?${qs}` : '/Infrastructure/Routes');
  },

  getTrips: (params?: { routeId?: number; date?: string; status?: string }) => {
    const qs = buildQuery(params);
    return api.get<TripDto[]>(qs ? `/Infrastructure/Trips?${qs}` : '/Infrastructure/Trips');
  },

  getTripById: (id: number) => api.get<TripDto>(`/Infrastructure/Trips/${id}`),

  getFares: (params?: { routeId?: number; coachClass?: number }) => {
    const qs = buildQuery(params);
    return api.get<FareDto[]>(qs ? `/Infrastructure/Fares?${qs}` : '/Infrastructure/Fares');
  },

  // Per-segment seat availability for a given trip. Returns the train's seat grid
  // annotated with `isAvailable` per seat for the requested boarding→alighting leg.
  getSegmentSeats: (tripId: number, boardingStationId: number, alightingStationId: number) => {
    const qs = buildQuery({ boardingStationId, alightingStationId });
    return api.get<SegmentSeatsDto>(`/Infrastructure/Trips/${tripId}/Seats?${qs}`);
  },

  // Resolved fare for a specific trip + segment. Backend picks trip > segment
  // > route. When `coachClass` is omitted the resolver returns the cheapest
  // fare across any class (search "starting from"); when supplied it's an
  // exact-class lookup (booking flow). Response carries the breakdown.
  getApplicableFare: (
    tripId: number,
    boardingStationId: number,
    alightingStationId: number,
    coachClass?: number,
  ) => {
    const qs = buildQuery({ boardingStationId, alightingStationId, coachClass });
    return api.get<FareDto>(`/Infrastructure/Trips/${tripId}/Fare?${qs}`);
  },
};

export { API_BASE_URL };
