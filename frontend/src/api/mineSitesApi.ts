import apiClient from './axios';
import type { MineSite, MineArea } from '../types';

export interface CreateMineSiteRequest {
  name: string;
  code?: string;
  mineType: string;
  jurisdiction: string;
  jurisdictionDetails?: string;
  latitude?: number;
  longitude?: number;
  address?: string;
  country?: string;
  state?: string;
  mineralsMined?: string;
  operatingCompany?: string;
  miningLicenseNumber?: string;
  licenseExpiryDate?: string;
  operationalSince?: string;
  status?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  nearestHospital?: string;
  nearestHospitalPhone?: string;
  nearestHospitalDistanceKm?: number;
  unitSystem?: string;
  timeZone?: string;
  shiftsPerDay?: number;
  shiftPattern?: string;
}

export interface UpdateMineSiteRequest extends CreateMineSiteRequest {
  id: string;
  status: string;
  unitSystem: string;
  timeZone: string;
  shiftsPerDay: number;
}

export interface CreateMineAreaRequest {
  mineSiteId: string;
  name: string;
  code?: string;
  areaType: string;
  description?: string;
  elevation?: number;
  isActive?: boolean;
  parentAreaId?: string;
  sortOrder?: number;
}

export interface UpdateMineAreaRequest {
  id: string;
  name: string;
  code?: string;
  areaType: string;
  description?: string;
  elevation?: number;
  isActive: boolean;
  parentAreaId?: string;
  sortOrder: number;
}

export const mineSitesApi = {
  getMineSites: async (): Promise<MineSite[]> => {
    const response = await apiClient.get<MineSite[]>('/minesites');
    return response.data;
  },

  getMineSiteById: async (id: string): Promise<MineSite> => {
    const response = await apiClient.get<MineSite>(`/minesites/${id}`);
    return response.data;
  },

  createMineSite: async (data: CreateMineSiteRequest): Promise<string> => {
    const response = await apiClient.post<string>('/minesites', data);
    return response.data;
  },

  updateMineSite: async (id: string, data: UpdateMineSiteRequest): Promise<void> => {
    await apiClient.put(`/minesites/${id}`, data);
  },

  deleteMineSite: async (id: string): Promise<void> => {
    await apiClient.delete(`/minesites/${id}`);
  },

  // Mine Areas
  getMineAreas: async (mineSiteId: string): Promise<MineArea[]> => {
    const response = await apiClient.get<MineArea[]>(`/mineareas/${mineSiteId}`);
    return response.data;
  },

  createMineArea: async (data: CreateMineAreaRequest): Promise<string> => {
    const response = await apiClient.post<string>('/mineareas', data);
    return response.data;
  },

  updateMineArea: async (id: string, data: UpdateMineAreaRequest): Promise<void> => {
    await apiClient.put(`/mineareas/${id}`, data);
  },

  deleteMineArea: async (id: string): Promise<void> => {
    await apiClient.delete(`/mineareas/${id}`);
  },
};
