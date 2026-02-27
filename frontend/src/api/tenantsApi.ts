import apiClient from './axios';
import type { Tenant } from '../types';

export interface CreateTenantRequest {
  name: string;
  subdomain: string;
}

export interface UpdateTenantRequest {
  name: string;
  isActive: boolean;
}

export const tenantsApi = {
  getTenants: async (): Promise<Tenant[]> => {
    const response = await apiClient.get<Tenant[]>('/tenants');
    return response.data;
  },

  createTenant: async (data: CreateTenantRequest): Promise<string> => {
    const response = await apiClient.post<string>('/tenants', data);
    return response.data;
  },

  updateTenant: async (id: string, data: UpdateTenantRequest): Promise<void> => {
    await apiClient.put(`/tenants/${id}`, data);
  },
};
