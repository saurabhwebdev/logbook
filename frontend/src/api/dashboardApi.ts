import apiClient from './axios';

export interface DashboardStats {
  userCount: number;
  roleCount: number;
  departmentCount: number;
  auditLogCount: number;
  fileCount: number;
  reportCount: number;
  activeTaskCount: number;
  enabledFeatureFlagCount: number;
  activeApiKeyCount: number;
  recentActivity: RecentActivity[];
}

export interface RecentActivity {
  action: string;
  entityName: string;
  entityId: string;
  timestamp: string;
}

export interface MiningDashboardStats {
  totalMineSites: number;
  activeMineSites: number;
  totalIncidents: number;
  openIncidents: number;
  incidentsThisMonth: number;
  lostTimeDays: number;
  totalInspections: number;
  overdueInspections: number;
  openFindings: number;
  totalEquipment: number;
  operationalEquipment: number;
  underMaintenanceEquipment: number;
  overdueMaintenanceCount: number;
  totalPersonnel: number;
  activePersonnel: number;
  expiringCertifications: number;
  totalProductionTonnes: number;
  productionThisMonth: number;
  dispatchCount: number;
  dispatchThisMonth: number;
  totalBlasts: number;
  blastsThisMonth: number;
  totalExplosivesUsedKg: number;
  activePermits: number;
  pendingPermits: number;
  expiredPermits: number;
  environmentalExceedances: number;
  openEnvironmentalIncidents: number;
  gasExceedances: number;
  criticalVentilationReadings: number;
  totalRequirements: number;
  compliantCount: number;
  nonCompliantCount: number;
  overdueAudits: number;
  unstableAssessments: number;
  pendingSurveys: number;
  recentSafetyIncidents: RecentMiningActivity[];
  recentInspections: RecentMiningActivity[];
  recentPermits: RecentMiningActivity[];
}

export interface RecentMiningActivity {
  id: string;
  title: string;
  status: string;
  severity: string;
  date: string;
}

export const dashboardApi = {
  getStats: async (): Promise<DashboardStats> => {
    const response = await apiClient.get<DashboardStats>('/dashboard/stats');
    return response.data;
  },

  getMiningStats: async (): Promise<MiningDashboardStats> => {
    const response = await apiClient.get<MiningDashboardStats>('/dashboard/mining-stats');
    return response.data;
  },
};
