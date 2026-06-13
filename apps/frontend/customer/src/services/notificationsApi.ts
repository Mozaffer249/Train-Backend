import { api } from './api';

export interface NotificationDto {
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

export const notificationsApi = {
  mine: (unreadOnly?: boolean) => {
    const qs = unreadOnly ? '?unreadOnly=true' : '';
    return api.get<NotificationDto[]>(`/Notifications/Mine${qs}`);
  },

  markRead: (id: number) => api.post<string>(`/Notifications/${id}/Read`, {}),
};
