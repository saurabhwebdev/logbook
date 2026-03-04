import apiClient from './axios';
import type { ShiftDefinition, ShiftInstance, ShiftHandover } from '../types';

export interface CreateShiftDefinitionRequest {
  mineSiteId: string;
  name: string;
  code?: string;
  startTime: string;
  endTime: string;
  shiftOrder?: number;
  color?: string;
  isActive?: boolean;
}

export interface UpdateShiftDefinitionRequest {
  id: string;
  name: string;
  code?: string;
  startTime: string;
  endTime: string;
  shiftOrder: number;
  color?: string;
  isActive: boolean;
}

export interface CreateShiftInstanceRequest {
  shiftDefinitionId: string;
  mineSiteId: string;
  date: string;
  supervisorName?: string;
  supervisorId?: string;
  status?: string;
  actualStartTime?: string;
  actualEndTime?: string;
  personnelCount?: number;
  weatherConditions?: string;
  notes?: string;
}

export interface UpdateShiftInstanceRequest {
  id: string;
  shiftDefinitionId: string;
  mineSiteId: string;
  date: string;
  supervisorName?: string;
  supervisorId?: string;
  status: string;
  actualStartTime?: string;
  actualEndTime?: string;
  personnelCount?: number;
  weatherConditions?: string;
  notes?: string;
}

export interface CreateShiftHandoverRequest {
  outgoingShiftInstanceId: string;
  incomingShiftInstanceId?: string;
  mineSiteId: string;
  handoverDateTime: string;
  safetyIssues?: string;
  ongoingOperations?: string;
  pendingTasks?: string;
  equipmentStatus?: string;
  environmentalConditions?: string;
  generalRemarks?: string;
  handedOverBy?: string;
  receivedBy?: string;
}

export interface UpdateShiftHandoverRequest {
  id: string;
  outgoingShiftInstanceId: string;
  incomingShiftInstanceId?: string;
  mineSiteId: string;
  handoverDateTime: string;
  safetyIssues?: string;
  ongoingOperations?: string;
  pendingTasks?: string;
  equipmentStatus?: string;
  environmentalConditions?: string;
  generalRemarks?: string;
  handedOverBy?: string;
  receivedBy?: string;
  status: string;
}

export const shiftsApi = {
  // Shift Definitions
  getShiftDefinitions: async (mineSiteId: string): Promise<ShiftDefinition[]> => {
    const response = await apiClient.get<ShiftDefinition[]>(`/shiftdefinitions/${mineSiteId}`);
    return response.data;
  },

  createShiftDefinition: async (data: CreateShiftDefinitionRequest): Promise<string> => {
    const response = await apiClient.post<string>('/shiftdefinitions', data);
    return response.data;
  },

  updateShiftDefinition: async (id: string, data: UpdateShiftDefinitionRequest): Promise<void> => {
    await apiClient.put(`/shiftdefinitions/${id}`, data);
  },

  deleteShiftDefinition: async (id: string): Promise<void> => {
    await apiClient.delete(`/shiftdefinitions/${id}`);
  },

  // Shift Instances
  getShiftInstances: async (mineSiteId: string, fromDate?: string, toDate?: string): Promise<ShiftInstance[]> => {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    const query = params.toString() ? `?${params.toString()}` : '';
    const response = await apiClient.get<ShiftInstance[]>(`/shiftinstances/${mineSiteId}${query}`);
    return response.data;
  },

  getShiftInstanceById: async (id: string): Promise<ShiftInstance> => {
    const response = await apiClient.get<ShiftInstance>(`/shiftinstances/detail/${id}`);
    return response.data;
  },

  createShiftInstance: async (data: CreateShiftInstanceRequest): Promise<string> => {
    const response = await apiClient.post<string>('/shiftinstances', data);
    return response.data;
  },

  updateShiftInstance: async (id: string, data: UpdateShiftInstanceRequest): Promise<void> => {
    await apiClient.put(`/shiftinstances/${id}`, data);
  },

  deleteShiftInstance: async (id: string): Promise<void> => {
    await apiClient.delete(`/shiftinstances/${id}`);
  },

  // Shift Handovers
  getShiftHandovers: async (mineSiteId: string, fromDate?: string, toDate?: string): Promise<ShiftHandover[]> => {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    const query = params.toString() ? `?${params.toString()}` : '';
    const response = await apiClient.get<ShiftHandover[]>(`/shifthandovers/${mineSiteId}${query}`);
    return response.data;
  },

  createShiftHandover: async (data: CreateShiftHandoverRequest): Promise<string> => {
    const response = await apiClient.post<string>('/shifthandovers', data);
    return response.data;
  },

  updateShiftHandover: async (id: string, data: UpdateShiftHandoverRequest): Promise<void> => {
    await apiClient.put(`/shifthandovers/${id}`, data);
  },

  acknowledgeShiftHandover: async (id: string): Promise<void> => {
    await apiClient.put(`/shifthandovers/${id}/acknowledge`);
  },
};
