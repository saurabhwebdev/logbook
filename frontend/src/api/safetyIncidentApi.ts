import apiClient from './axios';
import type { SafetyIncident, IncidentInvestigation } from '../types';

export interface CreateSafetyIncidentRequest {
  mineSiteId: string;
  mineAreaId?: string;
  title: string;
  incidentType: string;
  severity?: string;
  incidentDateTime: string;
  location: string;
  description: string;
  immediateActions?: string;
  reportedBy: string;
  injuredPersonName?: string;
  injuredPersonRole?: string;
  injuryType?: string;
  bodyPartAffected?: string;
  lostTimeDays?: number;
  isReportable?: boolean;
  regulatoryReference?: string;
  witnessNames?: string;
  rootCause?: string;
  contributingFactors?: string;
  correctiveActions?: string;
  correctiveActionDueDate?: string;
}

export interface UpdateSafetyIncidentRequest {
  id: string;
  title: string;
  incidentType: string;
  severity: string;
  incidentDateTime: string;
  location: string;
  description: string;
  immediateActions?: string;
  injuredPersonName?: string;
  injuredPersonRole?: string;
  injuryType?: string;
  bodyPartAffected?: string;
  lostTimeDays?: number;
  isReportable: boolean;
  regulatoryReference?: string;
  witnessNames?: string;
  rootCause?: string;
  contributingFactors?: string;
  correctiveActions?: string;
  correctiveActionDueDate?: string;
  correctiveActionCompletedDate?: string;
  status: string;
}

export interface CreateInvestigationRequest {
  safetyIncidentId: string;
  investigatorName: string;
  investigationDate: string;
  methodology: string;
  findings: string;
  rootCauseAnalysis?: string;
  recommendations?: string;
  preventiveMeasures?: string;
  evidenceReferences?: string;
}

export const safetyIncidentApi = {
  getAll: async (mineSiteId?: string, status?: string, severity?: string): Promise<SafetyIncident[]> => {
    const params = new URLSearchParams();
    if (mineSiteId) params.append('mineSiteId', mineSiteId);
    if (status) params.append('status', status);
    if (severity) params.append('severity', severity);
    const response = await apiClient.get<SafetyIncident[]>(`/safetyincidents?${params}`);
    return response.data;
  },

  getById: async (id: string): Promise<SafetyIncident> => {
    const response = await apiClient.get<SafetyIncident>(`/safetyincidents/${id}`);
    return response.data;
  },

  create: async (data: CreateSafetyIncidentRequest): Promise<string> => {
    const response = await apiClient.post<string>('/safetyincidents', data);
    return response.data;
  },

  update: async (id: string, data: UpdateSafetyIncidentRequest): Promise<void> => {
    await apiClient.put(`/safetyincidents/${id}`, data);
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/safetyincidents/${id}`);
  },

  getInvestigations: async (incidentId: string): Promise<IncidentInvestigation[]> => {
    const response = await apiClient.get<IncidentInvestigation[]>(`/safetyincidents/${incidentId}/investigations`);
    return response.data;
  },

  createInvestigation: async (incidentId: string, data: CreateInvestigationRequest): Promise<string> => {
    const response = await apiClient.post<string>(`/safetyincidents/${incidentId}/investigations`, data);
    return response.data;
  },
};
