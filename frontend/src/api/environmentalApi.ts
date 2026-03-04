import apiClient from './axios';
import type { EnvironmentalReading, EnvironmentalIncident } from '../types';

export interface CreateEnvironmentalReadingRequest {
  mineSiteId: string;
  mineAreaId?: string;
  readingType: string;
  parameter: string;
  value: number;
  unit: string;
  thresholdMin?: number;
  thresholdMax?: number;
  isExceedance: boolean;
  readingDateTime: string;
  monitoringStation?: string;
  instrumentUsed?: string;
  calibratedDate?: string;
  recordedBy: string;
  weatherConditions?: string;
  notes?: string;
}

export interface UpdateEnvironmentalReadingRequest {
  id: string;
  readingType: string;
  parameter: string;
  value: number;
  unit: string;
  thresholdMin?: number;
  thresholdMax?: number;
  isExceedance: boolean;
  readingDateTime: string;
  monitoringStation?: string;
  instrumentUsed?: string;
  calibratedDate?: string;
  recordedBy: string;
  weatherConditions?: string;
  notes?: string;
  status: string;
}

export interface CreateEnvironmentalIncidentRequest {
  mineSiteId: string;
  title: string;
  incidentType: string;
  severity: string;
  occurredAt: string;
  location: string;
  description: string;
  impactAssessment?: string;
  containmentActions?: string;
  remediationPlan?: string;
  reportedBy: string;
  notifiedAuthority: boolean;
  authorityReference?: string;
}

export interface UpdateEnvironmentalIncidentRequest {
  id: string;
  title: string;
  incidentType: string;
  severity: string;
  occurredAt: string;
  location: string;
  description: string;
  impactAssessment?: string;
  containmentActions?: string;
  remediationPlan?: string;
  reportedBy: string;
  notifiedAuthority: boolean;
  authorityReference?: string;
  status: string;
  closedAt?: string;
  closureNotes?: string;
}

export const environmentalApi = {
  // Readings
  getReadings: async (mineSiteId?: string, readingType?: string, status?: string): Promise<EnvironmentalReading[]> => {
    const params = new URLSearchParams();
    if (mineSiteId) params.append('mineSiteId', mineSiteId);
    if (readingType) params.append('readingType', readingType);
    if (status) params.append('status', status);
    const response = await apiClient.get<EnvironmentalReading[]>(`/environmental/readings?${params}`);
    return response.data;
  },

  createReading: async (data: CreateEnvironmentalReadingRequest): Promise<string> => {
    const response = await apiClient.post<string>('/environmental/readings', data);
    return response.data;
  },

  updateReading: async (id: string, data: UpdateEnvironmentalReadingRequest): Promise<void> => {
    await apiClient.put(`/environmental/readings/${id}`, data);
  },

  deleteReading: async (id: string): Promise<void> => {
    await apiClient.delete(`/environmental/readings/${id}`);
  },

  // Incidents
  getIncidents: async (mineSiteId?: string, status?: string): Promise<EnvironmentalIncident[]> => {
    const params = new URLSearchParams();
    if (mineSiteId) params.append('mineSiteId', mineSiteId);
    if (status) params.append('status', status);
    const response = await apiClient.get<EnvironmentalIncident[]>(`/environmental/incidents?${params}`);
    return response.data;
  },

  createIncident: async (data: CreateEnvironmentalIncidentRequest): Promise<string> => {
    const response = await apiClient.post<string>('/environmental/incidents', data);
    return response.data;
  },

  updateIncident: async (id: string, data: UpdateEnvironmentalIncidentRequest): Promise<void> => {
    await apiClient.put(`/environmental/incidents/${id}`, data);
  },
};
