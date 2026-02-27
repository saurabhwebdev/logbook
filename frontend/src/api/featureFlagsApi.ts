import apiClient from './axios';
import type { FeatureFlag } from '../types';

export const featureFlagsApi = {
  getAll: async (): Promise<FeatureFlag[]> => {
    const response = await apiClient.get<FeatureFlag[]>('/featureflags');
    return response.data;
  },

  toggle: async (id: string, isEnabled: boolean): Promise<void> => {
    await apiClient.put(`/featureflags/${id}/toggle`, { isEnabled });
  },
};
