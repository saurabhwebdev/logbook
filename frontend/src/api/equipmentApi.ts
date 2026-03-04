import apiClient from './axios';
import type { EquipmentItem, MaintenanceRecord } from '../types';

export interface CreateEquipmentRequest {
  mineSiteId: string;
  mineAreaId?: string;
  name: string;
  category: string;
  make?: string;
  model?: string;
  serialNumber?: string;
  yearOfManufacture?: number;
  purchaseDate?: string;
  purchaseCost?: number;
  location?: string;
  operatorName?: string;
  hoursOperated?: number;
  nextServiceHours?: number;
  nextServiceDate?: string;
  warrantyInfo?: string;
  notes?: string;
}

export interface UpdateEquipmentRequest {
  id: string;
  name: string;
  category: string;
  make?: string;
  model?: string;
  serialNumber?: string;
  yearOfManufacture?: number;
  purchaseCost?: number;
  status: string;
  location?: string;
  operatorName?: string;
  hoursOperated?: number;
  nextServiceHours?: number;
  nextServiceDate?: string;
  lastServiceDate?: string;
  warrantyInfo?: string;
  notes?: string;
}

export interface CreateMaintenanceRequest {
  equipmentId: string;
  maintenanceType: string;
  priority: string;
  title: string;
  description?: string;
  scheduledDate: string;
  performedBy?: string;
  notes?: string;
}

export interface UpdateMaintenanceRequest {
  id: string;
  maintenanceType: string;
  priority: string;
  title: string;
  description?: string;
  scheduledDate: string;
  startedAt?: string;
  completedAt?: string;
  performedBy?: string;
  status: string;
  downtimeHours?: number;
  laborCost?: number;
  partsCost?: number;
  partsUsed?: string;
  findings?: string;
  actionsTaken?: string;
  notes?: string;
}

export const equipmentApi = {
  getAll: async (mineSiteId?: string, category?: string, status?: string): Promise<EquipmentItem[]> => {
    const params = new URLSearchParams();
    if (mineSiteId) params.append('mineSiteId', mineSiteId);
    if (category) params.append('category', category);
    if (status) params.append('status', status);
    const response = await apiClient.get<EquipmentItem[]>(`/equipment?${params}`);
    return response.data;
  },

  getById: async (id: string): Promise<EquipmentItem> => {
    const response = await apiClient.get<EquipmentItem>(`/equipment/${id}`);
    return response.data;
  },

  create: async (data: CreateEquipmentRequest): Promise<string> => {
    const response = await apiClient.post<string>('/equipment', data);
    return response.data;
  },

  update: async (id: string, data: UpdateEquipmentRequest): Promise<void> => {
    await apiClient.put(`/equipment/${id}`, data);
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/equipment/${id}`);
  },

  // Maintenance
  getMaintenanceRecords: async (equipmentId?: string, status?: string): Promise<MaintenanceRecord[]> => {
    const params = new URLSearchParams();
    if (equipmentId) params.append('equipmentId', equipmentId);
    if (status) params.append('status', status);
    const response = await apiClient.get<MaintenanceRecord[]>(`/equipment/maintenance?${params}`);
    return response.data;
  },

  createMaintenance: async (equipmentId: string, data: CreateMaintenanceRequest): Promise<string> => {
    const response = await apiClient.post<string>(`/equipment/${equipmentId}/maintenance`, data);
    return response.data;
  },

  updateMaintenance: async (id: string, data: UpdateMaintenanceRequest): Promise<void> => {
    await apiClient.put(`/equipment/maintenance/${id}`, data);
  },
};
