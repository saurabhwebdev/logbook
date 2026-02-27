import apiClient from './axios';
import type {
  User,
  PaginatedResult,
  CreateUserRequest,
  UpdateUserRequest,
} from '../types';

export interface GetUsersParams {
  pageNumber: number;
  pageSize: number;
  searchTerm?: string;
  status?: string;
}

export const usersApi = {
  getUsers: async (
    params: GetUsersParams
  ): Promise<PaginatedResult<User>> => {
    const response = await apiClient.get<PaginatedResult<User>>('/users', {
      params,
    });
    return response.data;
  },

  getUserById: async (id: string): Promise<User> => {
    const response = await apiClient.get<User>(`/users/${id}`);
    return response.data;
  },

  createUser: async (data: CreateUserRequest): Promise<User> => {
    const response = await apiClient.post<User>('/users', data);
    return response.data;
  },

  updateUser: async (id: string, data: UpdateUserRequest): Promise<User> => {
    const response = await apiClient.put<User>(`/users/${id}`, data);
    return response.data;
  },

  deleteUser: async (id: string): Promise<void> => {
    await apiClient.delete(`/users/${id}`);
  },
};
