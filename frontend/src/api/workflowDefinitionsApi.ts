import api from './axios';
import type { WorkflowDefinition } from '../types';

export interface PaginatedResponse<T> {
  items: T[];  // Backend returns 'Items' but gets lowercased in JSON
  pageNumber: number;
  totalPages: number;
  totalCount: number;
}

export const workflowDefinitionsApi = {
  getAll: async (
    searchTerm?: string,
    category?: string,
    isActive?: boolean,
    pageNumber = 1,
    pageSize = 10
  ): Promise<PaginatedResponse<WorkflowDefinition>> => {
    const params = new URLSearchParams();
    if (searchTerm) params.append('searchTerm', searchTerm);
    if (category) params.append('category', category);
    if (isActive !== undefined) params.append('isActive', String(isActive));
    params.append('pageNumber', String(pageNumber));
    params.append('pageSize', String(pageSize));

    const { data } = await api.get<PaginatedResponse<WorkflowDefinition>>(
      `/workflow-definitions?${params.toString()}`
    );
    return data;
  },

  create: async (definition: Partial<WorkflowDefinition>): Promise<string> => {
    const { data } = await api.post<string>('/workflow-definitions', definition);
    return data;
  },
};
