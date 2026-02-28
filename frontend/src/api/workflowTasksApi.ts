import api from './axios';
import type { WorkflowTask } from '../types';

export interface PaginatedResponse<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export const workflowTasksApi = {
  getMyTasks: async (
    status?: string,
    pageNumber = 1,
    pageSize = 10
  ): Promise<PaginatedResponse<WorkflowTask>> => {
    const params = new URLSearchParams();
    if (status) params.append('status', status);
    params.append('pageNumber', String(pageNumber));
    params.append('pageSize', String(pageSize));

    const { data } = await api.get<PaginatedResponse<WorkflowTask>>(
      `/WorkflowTasks/my-tasks?${params.toString()}`
    );
    return data;
  },

  getById: async (taskId: string): Promise<WorkflowTask> => {
    const { data } = await api.get<WorkflowTask>(`/WorkflowTasks/${taskId}`);
    return data;
  },

  complete: async (taskId: string, status: string, comments?: string): Promise<void> => {
    await api.post(`/WorkflowTasks/${taskId}/complete`, { status, comments });
  },

  reassign: async (taskId: string, newAssigneeUserId: string): Promise<void> => {
    await api.post(`/WorkflowTasks/${taskId}/reassign`, { newAssigneeUserId });
  },
};
