import apiClient from './axios';
import type { Tenant } from '../types';

export const tenantsApi = {
  getTenants: async (): Promise<Tenant[]> => {
    const response = await apiClient.get<Tenant[]>('/tenants');
    return response.data;
  },
};
