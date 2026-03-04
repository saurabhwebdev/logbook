import apiClient from './axios';
import type { BlastEvent, ExplosiveUsage } from '../types';

export interface CreateBlastEventRequest {
  mineSiteId: string;
  mineAreaId?: string;
  title: string;
  blastType: string;
  scheduledDateTime: string;
  location: string;
  drillingPattern?: string;
  numberOfHoles?: number;
  totalExplosivesKg?: number;
  explosiveType?: string;
  detonatorType?: string;
  blastDesignNotes?: string;
  safetyRadius?: number;
  supervisorName: string;
  licensedBlasterName: string;
}

export interface UpdateBlastEventRequest {
  id: string;
  title: string;
  blastType: string;
  scheduledDateTime: string;
  actualDateTime?: string;
  location: string;
  drillingPattern?: string;
  numberOfHoles?: number;
  totalExplosivesKg?: number;
  explosiveType?: string;
  detonatorType?: string;
  status: string;
  blastDesignNotes?: string;
  safetyRadius?: number;
  evacuationConfirmed: boolean;
  sentryPostsConfirmed: boolean;
  preBlastWarningGiven: boolean;
  supervisorName: string;
  licensedBlasterName: string;
  vibrationReading?: number;
  airBlastReading?: number;
  postBlastInspection?: string;
  postBlastNotes?: string;
  fragmentationQuality?: string;
  misfireCount: number;
}

export interface CreateExplosiveUsageRequest {
  blastEventId: string;
  explosiveName: string;
  type: string;
  batchNumber?: string;
  quantityIssued: number;
  quantityUsed: number;
  quantityReturned: number;
  unit: string;
  magazineSource?: string;
  issuedBy?: string;
  receivedBy?: string;
  notes?: string;
}

export const blastingApi = {
  getAll: async (mineSiteId?: string, status?: string): Promise<BlastEvent[]> => {
    const params = new URLSearchParams();
    if (mineSiteId) params.append('mineSiteId', mineSiteId);
    if (status) params.append('status', status);
    const response = await apiClient.get<BlastEvent[]>(`/blasting?${params}`);
    return response.data;
  },

  getById: async (id: string): Promise<BlastEvent> => {
    const response = await apiClient.get<BlastEvent>(`/blasting/${id}`);
    return response.data;
  },

  create: async (data: CreateBlastEventRequest): Promise<string> => {
    const response = await apiClient.post<string>('/blasting', data);
    return response.data;
  },

  update: async (id: string, data: UpdateBlastEventRequest): Promise<void> => {
    await apiClient.put(`/blasting/${id}`, data);
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/blasting/${id}`);
  },

  getUsages: async (blastEventId: string): Promise<ExplosiveUsage[]> => {
    const response = await apiClient.get<ExplosiveUsage[]>(`/blasting/${blastEventId}/usages`);
    return response.data;
  },

  createUsage: async (blastEventId: string, data: CreateExplosiveUsageRequest): Promise<string> => {
    const response = await apiClient.post<string>(`/blasting/${blastEventId}/usages`, data);
    return response.data;
  },
};
