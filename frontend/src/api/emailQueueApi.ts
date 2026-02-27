import apiClient from './axios';
import type { EmailQueue } from '../types';

export const emailQueueApi = {
  getQueue: async (status?: number): Promise<EmailQueue[]> => {
    const response = await apiClient.get<EmailQueue[]>('/emailqueue', { params: { status } });
    return response.data;
  },

  send: async (data: { to: string; subject: string; htmlBody: string; plainTextBody?: string }): Promise<boolean> => {
    const response = await apiClient.post<boolean>('/emailqueue/send', data);
    return response.data;
  },

  queue: async (data: { to: string; subject: string; htmlBody: string; plainTextBody?: string }): Promise<void> => {
    await apiClient.post('/emailqueue/queue', data);
  },
};
