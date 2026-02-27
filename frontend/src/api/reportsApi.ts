import apiClient from './axios';
import type { ReportDefinition } from '../types';

export const reportsApi = {
  getAll: async (): Promise<ReportDefinition[]> => {
    const response = await apiClient.get<ReportDefinition[]>('/reports');
    return response.data;
  },

  create: async (data: {
    name: string;
    description?: string;
    entityType: string;
    columnsJson: string;
    filtersJson?: string;
    exportFormat: string;
  }): Promise<string> => {
    const response = await apiClient.post<string>('/reports', data);
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/reports/${id}`);
  },
};
