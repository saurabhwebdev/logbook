import apiClient from './axios';

export interface SearchResult {
  entityType: string;
  entityId: string;
  title: string;
  description: string | null;
  url: string;
}

export interface GlobalSearchResult {
  users: SearchResult[];
  roles: SearchResult[];
  departments: SearchResult[];
  files: SearchResult[];
  reports: SearchResult[];
  demoTasks: SearchResult[];
  helpArticles: SearchResult[];
}

export const searchApi = {
  globalSearch: async (searchTerm: string): Promise<GlobalSearchResult> => {
    const response = await apiClient.get<GlobalSearchResult>('/search', {
      params: { q: searchTerm },
    });
    return response.data;
  },
};
