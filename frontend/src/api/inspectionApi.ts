import apiClient from './axios';
import type { InspectionTemplate, Inspection, InspectionFinding } from '../types';

export interface CreateInspectionTemplateRequest {
  name: string;
  code: string;
  category: string;
  description?: string;
  checklistJson?: string;
  frequency: string;
  sortOrder: number;
}

export interface UpdateInspectionTemplateRequest extends CreateInspectionTemplateRequest {
  id: string;
  isActive: boolean;
}

export interface CreateInspectionRequest {
  inspectionTemplateId: string;
  mineSiteId: string;
  mineAreaId?: string;
  title: string;
  scheduledDate: string;
  inspectorName: string;
  inspectorRole?: string;
  weatherConditions?: string;
  personnelPresent?: number;
}

export interface UpdateInspectionRequest {
  id: string;
  title: string;
  scheduledDate: string;
  completedDate?: string;
  inspectorName: string;
  inspectorRole?: string;
  status: string;
  overallRating?: string;
  summary?: string;
  checklistResponsesJson?: string;
  weatherConditions?: string;
  personnelPresent?: number;
  signedOffBy?: string;
  signedOffAt?: string;
}

export interface CreateFindingRequest {
  inspectionId: string;
  category: string;
  severity: string;
  description: string;
  location?: string;
  recommendedAction?: string;
  assignedTo?: string;
  actionDueDate?: string;
}

export interface UpdateFindingRequest {
  id: string;
  category: string;
  severity: string;
  description: string;
  location?: string;
  recommendedAction?: string;
  assignedTo?: string;
  actionDueDate?: string;
  actionCompletedDate?: string;
  status: string;
  closureNotes?: string;
}

export const inspectionApi = {
  // Templates
  getTemplates: async (category?: string, isActive?: boolean): Promise<InspectionTemplate[]> => {
    const params = new URLSearchParams();
    if (category) params.append('category', category);
    if (isActive !== undefined) params.append('isActive', String(isActive));
    const response = await apiClient.get<InspectionTemplate[]>(`/inspections/templates?${params}`);
    return response.data;
  },

  createTemplate: async (data: CreateInspectionTemplateRequest): Promise<string> => {
    const response = await apiClient.post<string>('/inspections/templates', data);
    return response.data;
  },

  updateTemplate: async (id: string, data: UpdateInspectionTemplateRequest): Promise<void> => {
    await apiClient.put(`/inspections/templates/${id}`, data);
  },

  deleteTemplate: async (id: string): Promise<void> => {
    await apiClient.delete(`/inspections/templates/${id}`);
  },

  // Inspections
  getAll: async (mineSiteId?: string, templateId?: string, status?: string): Promise<Inspection[]> => {
    const params = new URLSearchParams();
    if (mineSiteId) params.append('mineSiteId', mineSiteId);
    if (templateId) params.append('templateId', templateId);
    if (status) params.append('status', status);
    const response = await apiClient.get<Inspection[]>(`/inspections?${params}`);
    return response.data;
  },

  getById: async (id: string): Promise<Inspection> => {
    const response = await apiClient.get<Inspection>(`/inspections/${id}`);
    return response.data;
  },

  create: async (data: CreateInspectionRequest): Promise<string> => {
    const response = await apiClient.post<string>('/inspections', data);
    return response.data;
  },

  update: async (id: string, data: UpdateInspectionRequest): Promise<void> => {
    await apiClient.put(`/inspections/${id}`, data);
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/inspections/${id}`);
  },

  // Findings
  getFindings: async (inspectionId: string): Promise<InspectionFinding[]> => {
    const response = await apiClient.get<InspectionFinding[]>(`/inspections/${inspectionId}/findings`);
    return response.data;
  },

  createFinding: async (inspectionId: string, data: CreateFindingRequest): Promise<string> => {
    const response = await apiClient.post<string>(`/inspections/${inspectionId}/findings`, data);
    return response.data;
  },

  updateFinding: async (id: string, data: UpdateFindingRequest): Promise<void> => {
    await apiClient.put(`/inspections/findings/${id}`, data);
  },
};
