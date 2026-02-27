import apiClient from './axios';
import type { FileMetadata } from '../types';

export const filesApi = {
  getAll: async (category?: string): Promise<FileMetadata[]> => {
    const params = category ? { category } : {};
    const response = await apiClient.get<FileMetadata[]>('/files', { params });
    return response.data;
  },

  upload: async (file: File, description?: string, category?: string): Promise<string> => {
    const formData = new FormData();
    formData.append('file', file);
    if (description) formData.append('description', description);
    if (category) formData.append('category', category);
    const response = await apiClient.post<string>('/files/upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/files/${id}`);
  },

  getDownloadUrl: (id: string): string => {
    return `http://localhost:5034/api/files/${id}/download`;
  },
};
