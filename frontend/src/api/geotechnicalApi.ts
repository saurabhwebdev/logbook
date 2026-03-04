import apiClient from './axios';
import type { GeotechnicalAssessment, SurveyRecord } from '../types';

export interface CreateGeotechnicalAssessmentRequest {
  mineSiteId: string;
  mineAreaId?: string;
  title: string;
  assessmentType: string;
  date: string;
  assessorName: string;
  location: string;
  rockMassRating?: number;
  slopeAngle?: number;
  waterTableDepth?: number;
  groundCondition?: string;
  stabilityStatus: string;
  recommendedActions?: string;
  monitoringRequired: boolean;
  nextAssessmentDate?: string;
  notes?: string;
}

export interface UpdateGeotechnicalAssessmentRequest {
  id: string;
  title: string;
  assessmentType: string;
  date: string;
  assessorName: string;
  location: string;
  rockMassRating?: number;
  slopeAngle?: number;
  waterTableDepth?: number;
  groundCondition?: string;
  stabilityStatus: string;
  recommendedActions?: string;
  monitoringRequired: boolean;
  nextAssessmentDate?: string;
  notes?: string;
  status: string;
}

export interface CreateSurveyRecordRequest {
  mineSiteId: string;
  mineAreaId?: string;
  title: string;
  surveyType: string;
  date: string;
  surveyorName: string;
  surveyorLicense?: string;
  location: string;
  easting?: number;
  northing?: number;
  elevation?: number;
  datum?: string;
  coordinateSystem?: string;
  equipmentUsed?: string;
  accuracy?: string;
  volumeCalculated?: number;
  areaCalculated?: number;
  findings?: string;
  notes?: string;
}

export interface UpdateSurveyRecordRequest {
  id: string;
  title: string;
  surveyType: string;
  date: string;
  surveyorName: string;
  surveyorLicense?: string;
  location: string;
  easting?: number;
  northing?: number;
  elevation?: number;
  datum?: string;
  coordinateSystem?: string;
  equipmentUsed?: string;
  accuracy?: string;
  volumeCalculated?: number;
  areaCalculated?: number;
  findings?: string;
  notes?: string;
  status: string;
}

export const geotechnicalApi = {
  // Assessments
  getAssessments: async (mineSiteId?: string, status?: string): Promise<GeotechnicalAssessment[]> => {
    const params = new URLSearchParams();
    if (mineSiteId) params.append('mineSiteId', mineSiteId);
    if (status) params.append('status', status);
    const response = await apiClient.get<GeotechnicalAssessment[]>(`/geotechnical/assessments?${params}`);
    return response.data;
  },

  createAssessment: async (data: CreateGeotechnicalAssessmentRequest): Promise<string> => {
    const response = await apiClient.post<string>('/geotechnical/assessments', data);
    return response.data;
  },

  updateAssessment: async (id: string, data: UpdateGeotechnicalAssessmentRequest): Promise<void> => {
    await apiClient.put(`/geotechnical/assessments/${id}`, data);
  },

  deleteAssessment: async (id: string): Promise<void> => {
    await apiClient.delete(`/geotechnical/assessments/${id}`);
  },

  // Surveys
  getSurveys: async (mineSiteId?: string, surveyType?: string): Promise<SurveyRecord[]> => {
    const params = new URLSearchParams();
    if (mineSiteId) params.append('mineSiteId', mineSiteId);
    if (surveyType) params.append('surveyType', surveyType);
    const response = await apiClient.get<SurveyRecord[]>(`/geotechnical/surveys?${params}`);
    return response.data;
  },

  createSurvey: async (data: CreateSurveyRecordRequest): Promise<string> => {
    const response = await apiClient.post<string>('/geotechnical/surveys', data);
    return response.data;
  },

  updateSurvey: async (id: string, data: UpdateSurveyRecordRequest): Promise<void> => {
    await apiClient.put(`/geotechnical/surveys/${id}`, data);
  },

  deleteSurvey: async (id: string): Promise<void> => {
    await apiClient.delete(`/geotechnical/surveys/${id}`);
  },
};
