import apiClient from './axios';
import type { AuditLog, PaginatedResult } from '../types';

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
};
