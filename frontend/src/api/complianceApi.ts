import apiClient from './axios';
import type { ComplianceRequirement, ComplianceAudit } from '../types';

export interface CreateComplianceRequirementRequest {
  mineSiteId: string;
  code: string;
  title: string;
  jurisdiction: string;
  category: string;
  description: string;
  regulatoryBody?: string;
  referenceDocument?: string;
  frequency: string;
  dueDate?: string;
  nextDueDate?: string;
  responsibleRole?: string;
  priority: string;
  penaltyForNonCompliance?: string;
  notes?: string;
}

export interface UpdateComplianceRequirementRequest {
  id: string;
  title: string;
  jurisdiction: string;
  category: string;
  description: string;
  regulatoryBody?: string;
  referenceDocument?: string;
  frequency: string;
  dueDate?: string;
  lastCompletedDate?: string;
  nextDueDate?: string;
  responsibleRole?: string;
  status: string;
  priority: string;
  penaltyForNonCompliance?: string;
  notes?: string;
  isActive: boolean;
}

export interface CreateComplianceAuditRequest {
  complianceRequirementId: string;
  auditDate: string;
  auditorName: string;
  auditType: string;
  findings: string;
  complianceStatus: string;
  correctiveActions?: string;
  actionDueDate?: string;
  evidenceReferences?: string;
  notes?: string;
}

export const complianceApi = {
  // Requirements
  getRequirements: async (mineSiteId?: string, status?: string, category?: string): Promise<ComplianceRequirement[]> => {
    const params = new URLSearchParams();
    if (mineSiteId) params.append('mineSiteId', mineSiteId);
    if (status) params.append('status', status);
    if (category) params.append('category', category);
    const response = await apiClient.get<ComplianceRequirement[]>(`/compliance/requirements?${params}`);
    return response.data;
  },

  createRequirement: async (data: CreateComplianceRequirementRequest): Promise<string> => {
    const response = await apiClient.post<string>('/compliance/requirements', data);
    return response.data;
  },

  updateRequirement: async (id: string, data: UpdateComplianceRequirementRequest): Promise<void> => {
    await apiClient.put(`/compliance/requirements/${id}`, data);
  },

  deleteRequirement: async (id: string): Promise<void> => {
    await apiClient.delete(`/compliance/requirements/${id}`);
  },

  // Audits
  getAudits: async (requirementId: string): Promise<ComplianceAudit[]> => {
    const response = await apiClient.get<ComplianceAudit[]>(`/compliance/requirements/${requirementId}/audits`);
    return response.data;
  },

  createAudit: async (data: CreateComplianceAuditRequest): Promise<string> => {
    const response = await apiClient.post<string>('/compliance/audits', data);
    return response.data;
  },
};
