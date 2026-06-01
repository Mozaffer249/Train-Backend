// Real backend Bookings API. Replaces the previous localStorage mock so seat
// inventory is shared across users (Option B per-segment availability).

import { api } from './api';
import type { BookingDto, CreateBookingPayload } from '../types/api';

export const bookingApi = {
  createBooking: (input: CreateBookingPayload) => api.post<BookingDto>('/Bookings', input),

  getMyBookings: () => api.get<BookingDto[]>('/Bookings/Mine'),

  getById: (id: number) => api.get<BookingDto>(`/Bookings/${id}`),

  cancelBooking: (id: number, reason?: string) =>
    api.post<string>(`/Bookings/${id}/Cancel`, { bookingId: id, reason }),
};
