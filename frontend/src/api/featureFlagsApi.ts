import apiClient from './axios';
import type { FeatureFlag } from '../types';

export interface CreateFeatureFlagRequest {
  name: string;
  description?: string;
  isEnabled: boolean;
}

export interface UpdateFeatureFlagRequest {
  name: string;
  description?: string;
}

export const featureFlagsApi = {
  getAll: async (): Promise<FeatureFlag[]> => {
    const response = await apiClient.get<FeatureFlag[]>('/featureflags');
    return response.data;
  },

  create: async (data: CreateFeatureFlagRequest): Promise<string> => {
    const response = await apiClient.post<string>('/featureflags', data);
    return response.data;
  },

  update: async (id: string, data: UpdateFeatureFlagRequest): Promise<void> => {
    await apiClient.put(`/featureflags/${id}`, data);
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/featureflags/${id}`);
  },

  toggle: async (id: string, isEnabled: boolean): Promise<void> => {
    await apiClient.put(`/featureflags/${id}/toggle`, { isEnabled });
  },
};
