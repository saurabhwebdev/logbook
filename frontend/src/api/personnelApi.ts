import apiClient from './axios';
import type { PersonnelRecord, PersonnelCertification } from '../types';

export interface CreatePersonnelRequest {
  mineSiteId: string;
  firstName: string;
  lastName: string;
  middleName?: string;
  role: string;
  department?: string;
  designation?: string;
  employmentType: string;
  dateOfJoining: string;
  contactPhone?: string;
  contactEmail?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  bloodGroup?: string;
  medicalFitnessCertificate?: string;
  medicalFitnessExpiry?: string;
  notes?: string;
}

export interface UpdatePersonnelRequest extends CreatePersonnelRequest {
  id: string;
  dateOfLeaving?: string;
  status: string;
}

export interface CreateCertificationRequest {
  personnelId: string;
  certificationName: string;
  certificateNumber?: string;
  issuingAuthority?: string;
  issueDate: string;
  expiryDate?: string;
  category?: string;
  notes?: string;
}

export const personnelApi = {
  getAll: async (mineSiteId?: string, status?: string, role?: string): Promise<PersonnelRecord[]> => {
    const params = new URLSearchParams();
    if (mineSiteId) params.append('mineSiteId', mineSiteId);
    if (status) params.append('status', status);
    if (role) params.append('role', role);
    const response = await apiClient.get<PersonnelRecord[]>(`/personnel?${params}`);
    return response.data;
  },

  create: async (data: CreatePersonnelRequest): Promise<string> => {
    const response = await apiClient.post<string>('/personnel', data);
    return response.data;
  },

  update: async (id: string, data: UpdatePersonnelRequest): Promise<void> => {
    await apiClient.put(`/personnel/${id}`, data);
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/personnel/${id}`);
  },

  getCertifications: async (personnelId: string): Promise<PersonnelCertification[]> => {
    const response = await apiClient.get<PersonnelCertification[]>(`/personnel/${personnelId}/certifications`);
    return response.data;
  },

  createCertification: async (personnelId: string, data: CreateCertificationRequest): Promise<string> => {
    const response = await apiClient.post<string>(`/personnel/${personnelId}/certifications`, data);
    return response.data;
  },
};
