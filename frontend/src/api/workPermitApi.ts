import apiClient from './axios';
import type { WorkPermit } from '../types';

export interface CreateWorkPermitRequest {
  mineSiteId: string;
  mineAreaId?: string;
  title: string;
  permitType: string;
  requestedBy: string;
  requestDate: string;
  startDateTime: string;
  endDateTime: string;
  location: string;
  workDescription: string;
  hazardsIdentified?: string;
  controlMeasures?: string;
  ppeRequired?: string;
  emergencyProcedures?: string;
  gasTestRequired: boolean;
  gasTestResults?: string;
  notes?: string;
}

export interface UpdateWorkPermitRequest {
  id: string;
  title: string;
  permitType: string;
  requestedBy: string;
  requestDate: string;
  startDateTime: string;
  endDateTime: string;
  location: string;
  workDescription: string;
  hazardsIdentified?: string;
  controlMeasures?: string;
  ppeRequired?: string;
  emergencyProcedures?: string;
  gasTestRequired: boolean;
  gasTestResults?: string;
  status: string;
  approvedBy?: string;
  approvedAt?: string;
  closedBy?: string;
  closedAt?: string;
  rejectionReason?: string;
  notes?: string;
}

export const workPermitApi = {
  getAll: async (mineSiteId?: string, status?: string, permitType?: string): Promise<WorkPermit[]> => {
    const params = new URLSearchParams();
    if (mineSiteId) params.append('mineSiteId', mineSiteId);
    if (status) params.append('status', status);
    if (permitType) params.append('permitType', permitType);
    const response = await apiClient.get<WorkPermit[]>(`/workpermits?${params}`);
    return response.data;
  },

  getById: async (id: string): Promise<WorkPermit> => {
    const response = await apiClient.get<WorkPermit>(`/workpermits/${id}`);
    return response.data;
  },

  create: async (data: CreateWorkPermitRequest): Promise<string> => {
    const response = await apiClient.post<string>('/workpermits', data);
    return response.data;
  },

  update: async (id: string, data: UpdateWorkPermitRequest): Promise<void> => {
    await apiClient.put(`/workpermits/${id}`, data);
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/workpermits/${id}`);
  },
};
