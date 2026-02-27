import apiClient from './axios';
import type { BackgroundJobStats } from '../types';

export const backgroundJobsApi = {
  getStats: async (): Promise<BackgroundJobStats> => {
    const response = await apiClient.get<BackgroundJobStats>('/backgroundjobs/stats');
    return response.data;
  },

  triggerProcessEmailQueue: async (): Promise<void> => {
    await apiClient.post('/backgroundjobs/trigger/process-email-queue');
  },

  triggerCleanupAuditLogs: async (): Promise<void> => {
    await apiClient.post('/backgroundjobs/trigger/cleanup-audit-logs');
  },
};
