import apiClient from './axios';
import type { StateDefinitionsResponse, TransitionLog } from '../types';

export interface CreateStateRequest {
  entityType: string;
  stateName: string;
  isInitial: boolean;
  isFinal: boolean;
  color?: string;
  sortOrder: number;
}

export interface UpdateStateRequest {
  stateName: string;
  isInitial: boolean;
  isFinal: boolean;
  color?: string;
  sortOrder: number;
}

export interface CreateTransitionRequest {
  entityType: string;
  fromState: string;
  toState: string;
  triggerName: string;
  requiredPermission?: string;
  description?: string;
}

export interface UpdateTransitionRequest {
  fromState: string;
  toState: string;
  triggerName: string;
  requiredPermission?: string;
  description?: string;
}

export const stateMachineApi = {
  getDefinitions: async (entityType: string): Promise<StateDefinitionsResponse> => {
    const response = await apiClient.get<StateDefinitionsResponse>(`/statemachine/${entityType}`);
    return response.data;
  },

  getTransitionLog: async (entityType: string, entityId: string): Promise<TransitionLog[]> => {
    const response = await apiClient.get<TransitionLog[]>(`/statemachine/${entityType}/${entityId}/log`);
    return response.data;
  },

  createState: async (data: CreateStateRequest): Promise<string> => {
    const response = await apiClient.post<string>('/statemachine/states', data);
    return response.data;
  },

  updateState: async (id: string, data: UpdateStateRequest): Promise<void> => {
    await apiClient.put(`/statemachine/states/${id}`, data);
  },

  deleteState: async (id: string): Promise<void> => {
    await apiClient.delete(`/statemachine/states/${id}`);
  },

  createTransition: async (data: CreateTransitionRequest): Promise<string> => {
    const response = await apiClient.post<string>('/statemachine/transitions', data);
    return response.data;
  },

  updateTransition: async (id: string, data: UpdateTransitionRequest): Promise<void> => {
    await apiClient.put(`/statemachine/transitions/${id}`, data);
  },

  deleteTransition: async (id: string): Promise<void> => {
    await apiClient.delete(`/statemachine/transitions/${id}`);
  },
};
