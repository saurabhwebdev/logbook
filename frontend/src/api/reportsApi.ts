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

  export: async (id: string): Promise<void> => {
    const response = await apiClient.get(`/reports/${id}/export`, { responseType: 'blob' });
    const contentDisposition = response.headers['content-disposition'];
    let fileName = 'report.xlsx';
    if (contentDisposition) {
      const match = contentDisposition.match(/filename="?([^";\n]+)"?/);
      if (match) fileName = match[1];
    }
    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.URL.revokeObjectURL(url);
  },
};
