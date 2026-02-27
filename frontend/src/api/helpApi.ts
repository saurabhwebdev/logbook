import apiClient from './axios';
import type { HelpArticle } from '../types';

export const helpApi = {
  getAll: async (params?: { category?: string; moduleKey?: string; publishedOnly?: boolean }): Promise<HelpArticle[]> => {
    const response = await apiClient.get<HelpArticle[]>('/help', { params });
    return response.data;
  },

  getBySlug: async (slug: string): Promise<HelpArticle | null> => {
    try {
      const response = await apiClient.get<HelpArticle>(`/help/${slug}`);
      return response.data;
    } catch {
      return null;
    }
  },

  create: async (data: {
    title: string;
    slug: string;
    moduleKey: string | null;
    content: string;
    category: string | null;
    sortOrder: number;
    isPublished: boolean;
    tags: string | null;
  }): Promise<string> => {
    const response = await apiClient.post<string>('/help', data);
    return response.data;
  },

  update: async (id: string, data: {
    id: string;
    title: string;
    slug: string;
    moduleKey: string | null;
    content: string;
    category: string | null;
    sortOrder: number;
    isPublished: boolean;
    tags: string | null;
  }): Promise<void> => {
    await apiClient.put(`/help/${id}`, data);
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/help/${id}`);
  },
};
