import apiClient from './axios';
import type { DemoTask, StateDefinitionsResponse, TransitionLog } from '../types';

export const demoTasksApi = {
  getAll: async (): Promise<DemoTask[]> => {
    const response = await apiClient.get<DemoTask[]>('/demotasks');
    return response.data;
  },

  create: async (data: { title: string; description?: string; assignedTo?: string; priority: string }): Promise<string> => {
    const response = await apiClient.post<string>('/demotasks', data);
    return response.data;
  },

  transition: async (id: string, triggerName: string, comments?: string): Promise<void> => {
    await apiClient.put(`/demotasks/${id}/transition`, { triggerName, comments });
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/demotasks/${id}`);
  },

  getHistory: async (id: string): Promise<TransitionLog[]> => {
    const response = await apiClient.get<TransitionLog[]>(`/demotasks/${id}/history`);
    return response.data;
  },

  getStates: async (): Promise<StateDefinitionsResponse> => {
    const response = await apiClient.get<StateDefinitionsResponse>('/demotasks/states');
    return response.data;
  },
};
