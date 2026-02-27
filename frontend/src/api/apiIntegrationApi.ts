import apiClient from './axios';
import type { ApiKeyInfo, ApiKeyCreateResponse, WebhookInfo, WebhookCreateResponse } from '../types';

export const apiIntegrationApi = {
  // API Keys
  getApiKeys: async (): Promise<ApiKeyInfo[]> => {
    const response = await apiClient.get<ApiKeyInfo[]>('/apiintegration/keys');
    return response.data;
  },

  createApiKey: async (data: { name: string; scopes?: string; expiresAt?: string }): Promise<ApiKeyCreateResponse> => {
    const response = await apiClient.post<ApiKeyCreateResponse>('/apiintegration/keys', data);
    return response.data;
  },

  revokeApiKey: async (id: string): Promise<void> => {
    await apiClient.put(`/apiintegration/keys/${id}/revoke`);
  },

  // Webhooks
  getWebhooks: async (): Promise<WebhookInfo[]> => {
    const response = await apiClient.get<WebhookInfo[]>('/apiintegration/webhooks');
    return response.data;
  },

  createWebhook: async (data: { name: string; endpointUrl: string; eventTypes: string }): Promise<WebhookCreateResponse> => {
    const response = await apiClient.post<WebhookCreateResponse>('/apiintegration/webhooks', data);
    return response.data;
  },

  deleteWebhook: async (id: string): Promise<void> => {
    await apiClient.delete(`/apiintegration/webhooks/${id}`);
  },
};
