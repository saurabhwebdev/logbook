import apiClient from './axios';

export interface DashboardStats {
  userCount: number;
  roleCount: number;
  departmentCount: number;
  auditLogCount: number;
  fileCount: number;
  reportCount: number;
  activeTaskCount: number;
  enabledFeatureFlagCount: number;
  activeApiKeyCount: number;
  recentActivity: RecentActivity[];
}

export interface RecentActivity {
  action: string;
  entityName: string;
  entityId: string;
  timestamp: string;
}

export const dashboardApi = {
  getStats: async (): Promise<DashboardStats> => {
    const response = await apiClient.get<DashboardStats>('/dashboard/stats');
    return response.data;
  },
};
