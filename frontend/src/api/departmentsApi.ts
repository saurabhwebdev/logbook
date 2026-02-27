import apiClient from './axios';
import type { Department } from '../types';

export interface CreateDepartmentRequest {
  name: string;
  description: string;
  code?: string;
  parentDepartmentId?: string;
}

export interface UpdateDepartmentRequest {
  name: string;
  description: string;
  code?: string;
  parentDepartmentId?: string;
}

export const departmentsApi = {
  getDepartments: async (): Promise<Department[]> => {
    const response = await apiClient.get<Department[]>('/departments');
    return response.data;
  },

  createDepartment: async (
    data: CreateDepartmentRequest
  ): Promise<Department> => {
    const response = await apiClient.post<Department>('/departments', data);
    return response.data;
  },

  updateDepartment: async (
    id: string,
    data: UpdateDepartmentRequest
  ): Promise<Department> => {
    const response = await apiClient.put<Department>(
      `/departments/${id}`,
      data
    );
    return response.data;
  },

  deleteDepartment: async (id: string): Promise<void> => {
    await apiClient.delete(`/departments/${id}`);
  },
};
