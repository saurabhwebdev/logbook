import apiClient from './axios';
import type { VentilationReading, GasReading } from '../types';

export interface CreateVentilationReadingRequest {
  mineSiteId: string;
  mineAreaId?: string;
  locationDescription: string;
  airflowVelocity?: number;
  airflowVolume?: number;
  temperature?: number;
  humidity?: number;
  barometricPressure?: number;
  readingDateTime: string;
  recordedBy: string;
  instrumentUsed?: string;
  doorStatus?: string;
  fanStatus?: string;
  notes?: string;
}

export interface UpdateVentilationReadingRequest {
  id: string;
  locationDescription: string;
  airflowVelocity?: number;
  airflowVolume?: number;
  temperature?: number;
  humidity?: number;
  barometricPressure?: number;
  readingDateTime: string;
  recordedBy: string;
  instrumentUsed?: string;
  doorStatus?: string;
  fanStatus?: string;
  ventilationStatus: string;
  notes?: string;
}

export interface CreateGasReadingRequest {
  mineSiteId: string;
  mineAreaId?: string;
  gasType: string;
  concentration: number;
  unit: string;
  thresholdTWA?: number;
  thresholdSTEL?: number;
  thresholdCeiling?: number;
  isExceedance: boolean;
  locationDescription: string;
  readingDateTime: string;
  recordedBy: string;
  instrumentId?: string;
  calibrationDate?: string;
  actionTaken?: string;
  notes?: string;
}

export interface UpdateGasReadingRequest {
  id: string;
  gasType: string;
  concentration: number;
  unit: string;
  thresholdTWA?: number;
  thresholdSTEL?: number;
  thresholdCeiling?: number;
  isExceedance: boolean;
  locationDescription: string;
  readingDateTime: string;
  recordedBy: string;
  instrumentId?: string;
  calibrationDate?: string;
  actionTaken?: string;
  status: string;
  notes?: string;
}

export const ventilationApi = {
  // Ventilation Readings
  getVentilationReadings: async (mineSiteId?: string, status?: string): Promise<VentilationReading[]> => {
    const params = new URLSearchParams();
    if (mineSiteId) params.append('mineSiteId', mineSiteId);
    if (status) params.append('status', status);
    const response = await apiClient.get<VentilationReading[]>(`/ventilation/readings?${params}`);
    return response.data;
  },

  createVentilationReading: async (data: CreateVentilationReadingRequest): Promise<string> => {
    const response = await apiClient.post<string>('/ventilation/readings', data);
    return response.data;
  },

  updateVentilationReading: async (id: string, data: UpdateVentilationReadingRequest): Promise<void> => {
    await apiClient.put(`/ventilation/readings/${id}`, data);
  },

  // Gas Readings
  getGasReadings: async (mineSiteId?: string, gasType?: string, status?: string): Promise<GasReading[]> => {
    const params = new URLSearchParams();
    if (mineSiteId) params.append('mineSiteId', mineSiteId);
    if (gasType) params.append('gasType', gasType);
    if (status) params.append('status', status);
    const response = await apiClient.get<GasReading[]>(`/ventilation/gas-readings?${params}`);
    return response.data;
  },

  createGasReading: async (data: CreateGasReadingRequest): Promise<string> => {
    const response = await apiClient.post<string>('/ventilation/gas-readings', data);
    return response.data;
  },

  updateGasReading: async (id: string, data: UpdateGasReadingRequest): Promise<void> => {
    await apiClient.put(`/ventilation/gas-readings/${id}`, data);
  },
};
