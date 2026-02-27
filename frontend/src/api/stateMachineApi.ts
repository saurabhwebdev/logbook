import apiClient from './axios';
import type { StateDefinitionsResponse, TransitionLog } from '../types';

export const stateMachineApi = {
  getDefinitions: async (entityType: string): Promise<StateDefinitionsResponse> => {
    const response = await apiClient.get<StateDefinitionsResponse>(`/statemachine/${entityType}`);
    return response.data;
  },

  getTransitionLog: async (entityType: string, entityId: string): Promise<TransitionLog[]> => {
    const response = await apiClient.get<TransitionLog[]>(`/statemachine/${entityType}/${entityId}/log`);
    return response.data;
  },
};
