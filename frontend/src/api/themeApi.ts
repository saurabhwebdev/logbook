import apiClient from './axios';

export interface TenantTheme {
  tenantName: string;
  logoUrl: string | null;
  primaryColor: string | null;
  sidebarColor: string | null;
}

export const themeApi = {
  getTheme: async (): Promise<TenantTheme> => {
    const response = await apiClient.get<TenantTheme>('/tenants/theme');
    return response.data;
  },

  updateTheme: async (data: { logoUrl?: string; primaryColor?: string; sidebarColor?: string }): Promise<void> => {
    await apiClient.put('/tenants/theme', data);
  },
};
