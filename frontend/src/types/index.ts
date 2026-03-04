export interface AuthUser {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  profilePhotoUrl?: string | null;
  tenantId: string;
  tenantName: string;
  roles: string[];
  permissions: string[];
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: AuthUser;
}

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber: string | null;
  profilePhotoUrl?: string | null;
  status: string;
  departmentId: string | null;
  departmentName: string | null;
  createdAt: string;
  lastLoginAt: string | null;
  roles: string[];
}

export interface Role {
  id: string;
  name: string;
  description: string;
  isSystemRole: boolean;
  createdAt: string;
  permissionCount: number;
  permissions: string[];
}

export interface Department {
  id: string;
  name: string;
  code: string | null;
  parentDepartmentId: string | null;
  parentDepartmentName: string | null;
  createdAt: string;
  userCount: number;
}

export interface AuditLog {
  id: string;
  userId: string;
  action: string;
  entityName: string;
  entityId: string;
  oldValues: string | null;
  newValues: string | null;
  ipAddress: string | null;
  timestamp: string;
}

export interface UserActivity {
  id: number;
  action: string;
  entityName: string;
  entityId: string;
  oldValues: string | null;
  newValues: string | null;
  timestamp: string;
}

export interface Tenant {
  id: string;
  name: string;
  subdomain: string;
  isActive: boolean;
  createdAt: string;
}

// Phase 2 types
export interface SystemConfiguration {
  id: string;
  key: string;
  value: string;
  category: string;
  description: string | null;
  dataType: string;
}

export interface FeatureFlag {
  id: string;
  name: string;
  description: string | null;
  isEnabled: boolean;
}

export interface Notification {
  id: string;
  title: string;
  message: string;
  type: string;
  link: string | null;
  isRead: boolean;
  createdAt: string;
}

export interface StateDefinition {
  id: string;
  entityType: string;
  stateName: string;
  isInitial: boolean;
  isFinal: boolean;
  color: string | null;
  sortOrder: number;
}

export interface StateTransitionDefinition {
  id: string;
  entityType: string;
  fromState: string;
  toState: string;
  triggerName: string;
  requiredPermission: string | null;
  description: string | null;
}

export interface TransitionLog {
  id: string;
  fromState: string;
  toState: string;
  triggerName: string;
  performedBy: string | null;
  comments: string | null;
  transitionedAt: string;
}

export interface StateDefinitionsResponse {
  states: StateDefinition[];
  transitions: StateTransitionDefinition[];
}

// Phase 3 types
export interface FileMetadata {
  id: string;
  fileName: string;
  originalFileName: string;
  contentType: string;
  fileSize: number;
  description: string | null;
  category: string | null;
  uploadedByName: string | null;
  createdAt: string;
}

export interface ReportDefinition {
  id: string;
  name: string;
  description: string | null;
  entityType: string;
  exportFormat: string;
  isActive: boolean;
  createdAt: string;
}

export interface ApiKeyInfo {
  id: string;
  name: string;
  keyPrefix: string;
  expiresAt: string | null;
  isActive: boolean;
  lastUsedAt: string | null;
  scopes: string | null;
  createdAt: string;
}

export interface ApiKeyCreateResponse {
  id: string;
  rawKey: string;
}

export interface WebhookInfo {
  id: string;
  name: string;
  endpointUrl: string;
  eventTypes: string;
  isActive: boolean;
  lastTriggeredAt: string | null;
  failureCount: number;
  createdAt: string;
}

export interface WebhookCreateResponse {
  id: string;
  secret: string;
}

export interface DemoTask {
  id: string;
  title: string;
  description: string | null;
  currentState: string;
  assignedTo: string | null;
  priority: string;
  createdAt: string;
}

// Help Module
export interface HelpArticle {
  id: string;
  title: string;
  slug: string;
  moduleKey: string | null;
  content: string;
  category: string | null;
  sortOrder: number;
  isPublished: boolean;
  tags: string | null;
  createdAt: string;
  modifiedAt: string | null;
}

export interface PaginatedResult<T> {
  items: T[];
  pageNumber: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface CreateUserRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  departmentId?: string;
  roleIds: string[];
}

export interface UpdateUserRequest {
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  departmentId?: string;
  status: string;
  roleIds: string[];
}

export interface EmailTemplate {
  id: string;
  name: string;
  subject: string;
  htmlBody: string;
  plainTextBody?: string;
  isActive: boolean;
  createdAt: string;
}

export type EmailStatus = 0 | 1 | 2; // 0 = Pending, 1 = Sent, 2 = Failed

export interface EmailQueue {
  id: string;
  to: string;
  subject: string;
  status: EmailStatus;
  sentAt?: string;
  failureReason?: string;
  retryCount: number;
  createdAt: string;
}

export interface BackgroundJobStats {
  succeededJobs: number;
  failedJobs: number;
  processingJobs: number;
  scheduledJobs: number;
  enqueuedJobs: number;
  serversCount: number;
  recurringJobsCount: number;
  deletedJobs: number;
}

// Workflow Engine types
export interface WorkflowDefinition {
  id: string;
  name: string;
  description: string;
  category: string;
  configurationJson: string;
  version: number;
  isActive: boolean;
  createdAt: string;
}

export interface WorkflowInstance {
  id: string;
  workflowDefinitionId: string;
  workflowDefinitionName: string;
  entityType: string;
  entityId: string;
  status: string;
  currentStepName: string;
  createdAt: string;
  completedAt?: string;
}

export interface WorkflowTask {
  id: string;
  workflowInstanceId: string;
  taskName: string;
  taskType: string;
  assignedToUserId: string;
  assignedToUserName: string;
  status: string;
  priority: number;
  dueDate?: string;
  completedAt?: string;
  completedByUserName?: string;
  comments?: string;
  createdAt: string;
  workflowDefinitionName: string;
  entityType: string;
  entityId: string;
}

export interface WorkflowStatistics {
  totalDefinitions: number;
  activeInstances: number;
  completedToday: number;
  pendingTasks: number;
}

export interface UserPresence {
  userId: string;
  userName: string;
  isOnline: boolean;
  lastSeen: string;
}

// ===== Logbook Mining Modules =====

export interface MineSite {
  id: string;
  name: string;
  code: string | null;
  mineType: string;
  jurisdiction: string;
  jurisdictionDetails: string | null;
  latitude: number | null;
  longitude: number | null;
  address: string | null;
  country: string | null;
  state: string | null;
  mineralsMined: string | null;
  operatingCompany: string | null;
  miningLicenseNumber: string | null;
  licenseExpiryDate: string | null;
  operationalSince: string | null;
  status: string;
  emergencyContactName: string | null;
  emergencyContactPhone: string | null;
  nearestHospital: string | null;
  nearestHospitalPhone: string | null;
  nearestHospitalDistanceKm: number | null;
  unitSystem: string;
  timeZone: string;
  shiftsPerDay: number;
  shiftPattern: string | null;
  createdAt: string;
  areaCount: number;
}

export interface MineArea {
  id: string;
  mineSiteId: string;
  mineSiteName: string;
  name: string;
  code: string | null;
  areaType: string;
  description: string | null;
  elevation: number | null;
  isActive: boolean;
  parentAreaId: string | null;
  parentAreaName: string | null;
  sortOrder: number;
  createdAt: string;
  childAreaCount: number;
}
