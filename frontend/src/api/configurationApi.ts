import apiClient from './axios';
import type { SystemConfiguration } from '../types';

export const configurationApi = {
  getAll: async (category?: string): Promise<SystemConfiguration[]> => {
    const params = category ? { category } : {};
    const response = await apiClient.get<SystemConfiguration[]>('/configuration', { params });
    return response.data;
  },

  upsert: async (data: { key: string; value: string; category: string; description?: string; dataType: string }): Promise<string> => {
    const response = await apiClient.post<string>('/configuration', data);
    return response.data;
  },
};
