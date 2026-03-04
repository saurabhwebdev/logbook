import apiClient from './axios';
import type { StatutoryRegister, RegisterEntry } from '../types';

export interface CreateStatutoryRegisterRequest {
  mineSiteId: string;
  name: string;
  code?: string;
  registerType: string;
  description?: string;
  jurisdiction: string;
  isRequired?: boolean;
  retentionYears?: number;
  isActive?: boolean;
  sortOrder?: number;
}

export interface UpdateStatutoryRegisterRequest {
  id: string;
  name: string;
  code?: string;
  registerType: string;
  description?: string;
  jurisdiction: string;
  isRequired: boolean;
  retentionYears: number;
  isActive: boolean;
  sortOrder: number;
}

export interface CreateRegisterEntryRequest {
  statutoryRegisterId: string;
  mineSiteId: string;
  entryDate: string;
  shiftInstanceId?: string;
  mineAreaId?: string;
  subject: string;
  details: string;
  reportedBy: string;
  witnessName?: string;
  actionTaken?: string;
  actionDueDate?: string;
  actionCompletedDate?: string;
}

export interface AmendRegisterEntryRequest {
  originalEntryId: string;
  statutoryRegisterId: string;
  mineSiteId: string;
  entryDate: string;
  subject: string;
  details: string;
  reportedBy: string;
  witnessName?: string;
  actionTaken?: string;
  actionDueDate?: string;
  actionCompletedDate?: string;
  amendmentReason: string;
}

export const statutoryRegistersApi = {
  // Registers
  getRegisters: async (mineSiteId: string): Promise<StatutoryRegister[]> => {
    const response = await apiClient.get<StatutoryRegister[]>(`/statutoryregisters/${mineSiteId}`);
    return response.data;
  },

  getRegisterById: async (id: string): Promise<StatutoryRegister> => {
    const response = await apiClient.get<StatutoryRegister>(`/statutoryregisters/detail/${id}`);
    return response.data;
  },

  createRegister: async (data: CreateStatutoryRegisterRequest): Promise<string> => {
    const response = await apiClient.post<string>('/statutoryregisters', data);
    return response.data;
  },

  updateRegister: async (id: string, data: UpdateStatutoryRegisterRequest): Promise<void> => {
    await apiClient.put(`/statutoryregisters/${id}`, data);
  },

  deleteRegister: async (id: string): Promise<void> => {
    await apiClient.delete(`/statutoryregisters/${id}`);
  },

  // Entries
  getEntries: async (
    statutoryRegisterId: string,
    status?: string,
    fromDate?: string,
    toDate?: string,
  ): Promise<RegisterEntry[]> => {
    const params = new URLSearchParams();
    if (status) params.append('status', status);
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    const query = params.toString() ? `?${params.toString()}` : '';
    const response = await apiClient.get<RegisterEntry[]>(`/registerentries/${statutoryRegisterId}${query}`);
    return response.data;
  },

  getEntryById: async (id: string): Promise<RegisterEntry> => {
    const response = await apiClient.get<RegisterEntry>(`/registerentries/detail/${id}`);
    return response.data;
  },

  createEntry: async (data: CreateRegisterEntryRequest): Promise<string> => {
    const response = await apiClient.post<string>('/registerentries', data);
    return response.data;
  },

  amendEntry: async (id: string, data: AmendRegisterEntryRequest): Promise<string> => {
    const response = await apiClient.post<string>(`/registerentries/${id}/amend`, data);
    return response.data;
  },
};
