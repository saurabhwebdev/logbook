import apiClient from './axios';
import type { Notification } from '../types';

export const notificationsApi = {
  getMy: async (unreadOnly = false): Promise<Notification[]> => {
    const response = await apiClient.get<Notification[]>('/notifications', { params: { unreadOnly } });
    return response.data;
  },

  markRead: async (id: string): Promise<void> => {
    await apiClient.put(`/notifications/${id}/read`);
  },

  markAllRead: async (): Promise<number> => {
    const response = await apiClient.put<number>('/notifications/read-all');
    return response.data;
  },

  send: async (data: { recipientUserId: string; title: string; message: string; type?: string; link?: string }): Promise<string> => {
    const response = await apiClient.post<string>('/notifications', data);
    return response.data;
  },
};
