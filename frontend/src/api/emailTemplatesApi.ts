import apiClient from './axios';
import type { EmailTemplate } from '../types';

export const emailTemplatesApi = {
  getAll: async (): Promise<EmailTemplate[]> => {
    const response = await apiClient.get<EmailTemplate[]>('/emailtemplates');
    return response.data;
  },

  getById: async (id: string): Promise<EmailTemplate> => {
    const response = await apiClient.get<EmailTemplate>(`/emailtemplates/${id}`);
    return response.data;
  },

  create: async (data: Partial<EmailTemplate>): Promise<string> => {
    const response = await apiClient.post<string>('/emailtemplates', data);
    return response.data;
  },

  update: async (id: string, data: Partial<EmailTemplate>): Promise<void> => {
    await apiClient.put(`/emailtemplates/${id}`, { ...data, id });
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/emailtemplates/${id}`);
  },
};
