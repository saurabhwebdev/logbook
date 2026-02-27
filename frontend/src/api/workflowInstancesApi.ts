import api from './axios';
import type { WorkflowInstance, WorkflowStatistics } from '../types';

export interface PaginatedResponse<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export const workflowInstancesApi = {
  getAll: async (
    workflowDefinitionId?: string,
    entityType?: string,
    status?: string,
    pageNumber = 1,
    pageSize = 10
  ): Promise<PaginatedResponse<WorkflowInstance>> => {
    const params = new URLSearchParams();
    if (workflowDefinitionId) params.append('workflowDefinitionId', workflowDefinitionId);
    if (entityType) params.append('entityType', entityType);
    if (status) params.append('status', status);
    params.append('pageNumber', String(pageNumber));
    params.append('pageSize', String(pageSize));

    const { data } = await api.get<PaginatedResponse<WorkflowInstance>>(
      `/workflow-instances?${params.toString()}`
    );
    return data;
  },

  getStatistics: async (): Promise<WorkflowStatistics> => {
    const { data } = await api.get<WorkflowStatistics>('/workflow-instances/statistics');
    return data;
  },

  start: async (workflowDefinitionId: string, entityType: string, entityId: string): Promise<string> => {
    const { data } = await api.post<string>('/workflow-instances', {
      workflowDefinitionId,
      entityType,
      entityId,
    });
    return data;
  },

  cancel: async (instanceId: string): Promise<void> => {
    await api.post(`/workflow-instances/${instanceId}/cancel`);
  },
};
