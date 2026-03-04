import apiClient from './axios';
import type { ProductionLog, DispatchRecord } from '../types';

export interface CreateProductionLogRequest {
  mineSiteId: string;
  mineAreaId?: string;
  shiftInstanceId?: string;
  date: string;
  shiftName?: string;
  material: string;
  sourceLocation?: string;
  destinationLocation?: string;
  quantityTonnes: number;
  quantityBCM?: number;
  equipmentUsed?: string;
  operatorName?: string;
  haulingDistance?: number;
  loadCount?: number;
  notes?: string;
}

export interface UpdateProductionLogRequest {
  id: string;
  mineAreaId?: string;
  shiftInstanceId?: string;
  date: string;
  shiftName?: string;
  material: string;
  sourceLocation?: string;
  destinationLocation?: string;
  quantityTonnes: number;
  quantityBCM?: number;
  equipmentUsed?: string;
  operatorName?: string;
  haulingDistance?: number;
  loadCount?: number;
  status: string;
  notes?: string;
  verifiedBy?: string;
  verifiedAt?: string;
}

export interface CreateDispatchRecordRequest {
  mineSiteId: string;
  date: string;
  vehicleNumber: string;
  driverName?: string;
  material: string;
  sourceLocation: string;
  destinationLocation: string;
  weighbridgeTicketNumber?: string;
  grossWeight?: number;
  tareWeight?: number;
  netWeight?: number;
  unit?: string;
  departureTime?: string;
  arrivalTime?: string;
  notes?: string;
}

export interface UpdateDispatchRecordRequest {
  id: string;
  date: string;
  vehicleNumber: string;
  driverName?: string;
  material: string;
  sourceLocation: string;
  destinationLocation: string;
  weighbridgeTicketNumber?: string;
  grossWeight?: number;
  tareWeight?: number;
  netWeight?: number;
  unit: string;
  departureTime?: string;
  arrivalTime?: string;
  status: string;
  notes?: string;
}

export const productionApi = {
  // Production Logs
  getLogs: async (mineSiteId?: string, dateFrom?: string, dateTo?: string, material?: string): Promise<ProductionLog[]> => {
    const params = new URLSearchParams();
    if (mineSiteId) params.append('mineSiteId', mineSiteId);
    if (dateFrom) params.append('dateFrom', dateFrom);
    if (dateTo) params.append('dateTo', dateTo);
    if (material) params.append('material', material);
    const response = await apiClient.get<ProductionLog[]>(`/production/logs?${params}`);
    return response.data;
  },

  createLog: async (data: CreateProductionLogRequest): Promise<string> => {
    const response = await apiClient.post<string>('/production/logs', data);
    return response.data;
  },

  updateLog: async (id: string, data: UpdateProductionLogRequest): Promise<void> => {
    await apiClient.put(`/production/logs/${id}`, data);
  },

  deleteLog: async (id: string): Promise<void> => {
    await apiClient.delete(`/production/logs/${id}`);
  },

  // Dispatch Records
  getDispatch: async (mineSiteId?: string, status?: string): Promise<DispatchRecord[]> => {
    const params = new URLSearchParams();
    if (mineSiteId) params.append('mineSiteId', mineSiteId);
    if (status) params.append('status', status);
    const response = await apiClient.get<DispatchRecord[]>(`/production/dispatch?${params}`);
    return response.data;
  },

  createDispatch: async (data: CreateDispatchRecordRequest): Promise<string> => {
    const response = await apiClient.post<string>('/production/dispatch', data);
    return response.data;
  },

  updateDispatch: async (id: string, data: UpdateDispatchRecordRequest): Promise<void> => {
    await apiClient.put(`/production/dispatch/${id}`, data);
  },

  deleteDispatch: async (id: string): Promise<void> => {
    await apiClient.delete(`/production/dispatch/${id}`);
  },
};
