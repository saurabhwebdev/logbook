import apiClient from './axios';

export const permissionsApi = {
  getPermissions: async (): Promise<string[]> => {
    const response = await apiClient.get<string[]>('/permissions');
    return response.data;
  },
};
