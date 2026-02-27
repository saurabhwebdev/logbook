import api from './axios';

export const presenceApi = {
  getOnlineUsers: async (): Promise<string[]> => {
    const { data } = await api.get<string[]>('/presence/online-users');
    return data;
  },

  isUserOnline: async (userId: string): Promise<boolean> => {
    const { data } = await api.get<boolean>(`/presence/is-online/${userId}`);
    return data;
  },
};
