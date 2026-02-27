import apiClient from './axios';
import type { AuditLog, PaginatedResult, UserActivity } from '../types';

export interface GetAuditLogsParams {
  pageNumber: number;
  pageSize: number;
  startDate?: string;
  endDate?: string;
  entityName?: string;
  userId?: string;
  action?: string;
}

export const auditLogsApi = {
  getAuditLogs: async (
    params: GetAuditLogsParams
  ): Promise<PaginatedResult<AuditLog>> => {
    const response = await apiClient.get<PaginatedResult<AuditLog>>(
      '/auditlogs',
      { params }
    );
    return response.data;
  },

  getUserActivity: async (limit: number = 50): Promise<UserActivity[]> => {
    const response = await apiClient.get<UserActivity[]>(
      '/auditlogs/my-activity',
      { params: { limit } }
    );
    return response.data;
  },
};
