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

// Shift Management
export interface ShiftDefinition {
  id: string;
  mineSiteId: string;
  mineSiteName: string;
  name: string;
  code: string | null;
  startTime: string;
  endTime: string;
  shiftOrder: number;
  color: string | null;
  isActive: boolean;
  createdAt: string;
  instanceCount: number;
}

export interface ShiftInstance {
  id: string;
  shiftDefinitionId: string;
  shiftDefinitionName: string;
  mineSiteId: string;
  mineSiteName: string;
  date: string;
  supervisorName: string | null;
  supervisorId: string | null;
  status: string;
  actualStartTime: string | null;
  actualEndTime: string | null;
  personnelCount: number | null;
  weatherConditions: string | null;
  notes: string | null;
  createdAt: string;
  handoverCount: number;
}

export interface ShiftHandover {
  id: string;
  outgoingShiftInstanceId: string;
  outgoingShiftName: string;
  incomingShiftInstanceId: string | null;
  incomingShiftName: string | null;
  mineSiteId: string;
  mineSiteName: string;
  handoverDateTime: string;
  safetyIssues: string | null;
  ongoingOperations: string | null;
  pendingTasks: string | null;
  equipmentStatus: string | null;
  environmentalConditions: string | null;
  generalRemarks: string | null;
  handedOverBy: string | null;
  receivedBy: string | null;
  status: string;
  acknowledgedAt: string | null;
  createdAt: string;
}

// Statutory Registers
export interface StatutoryRegister {
  id: string;
  mineSiteId: string;
  mineSiteName: string;
  name: string;
  code: string | null;
  registerType: string;
  description: string | null;
  jurisdiction: string;
  isRequired: boolean;
  retentionYears: number;
  isActive: boolean;
  sortOrder: number;
  createdAt: string;
  entryCount: number;
}

export interface RegisterEntry {
  id: string;
  statutoryRegisterId: string;
  registerName: string;
  mineSiteId: string;
  mineSiteName: string;
  entryNumber: number;
  entryDate: string;
  shiftInstanceId: string | null;
  mineAreaId: string | null;
  mineAreaName: string | null;
  subject: string;
  details: string;
  reportedBy: string;
  witnessName: string | null;
  actionTaken: string | null;
  actionDueDate: string | null;
  actionCompletedDate: string | null;
  status: string;
  amendmentOfEntryId: string | null;
  amendmentReason: string | null;
  createdAt: string;
  amendmentCount: number;
}

// Safety & Incident Management
export interface SafetyIncident {
  id: string;
  mineSiteId: string;
  mineSiteName: string;
  mineAreaId: string | null;
  mineAreaName: string | null;
  incidentNumber: string;
  title: string;
  incidentType: string;
  severity: string;
  incidentDateTime: string;
  location: string;
  description: string;
  immediateActions: string | null;
  reportedBy: string;
  reportedAt: string;
  injuredPersonName: string | null;
  injuredPersonRole: string | null;
  injuryType: string | null;
  bodyPartAffected: string | null;
  lostTimeDays: number | null;
  isReportable: boolean;
  regulatoryReference: string | null;
  witnessNames: string | null;
  rootCause: string | null;
  contributingFactors: string | null;
  correctiveActions: string | null;
  correctiveActionDueDate: string | null;
  correctiveActionCompletedDate: string | null;
  status: string;
  createdAt: string;
  investigationCount: number;
}

export interface IncidentInvestigation {
  id: string;
  safetyIncidentId: string;
  incidentTitle: string;
  investigatorName: string;
  investigationDate: string;
  methodology: string;
  findings: string;
  rootCauseAnalysis: string | null;
  recommendations: string | null;
  preventiveMeasures: string | null;
  evidenceReferences: string | null;
  status: string;
  createdAt: string;
}

// Inspection & Audit Management
export interface InspectionTemplate {
  id: string;
  name: string;
  code: string;
  category: string;
  description: string | null;
  checklistJson: string | null;
  frequency: string;
  isActive: boolean;
  sortOrder: number;
  inspectionCount: number;
  createdAt: string;
}

export interface Inspection {
  id: string;
  inspectionTemplateId: string;
  templateName: string;
  mineSiteId: string;
  mineSiteName: string;
  mineAreaId: string | null;
  mineAreaName: string | null;
  inspectionNumber: string;
  title: string;
  scheduledDate: string;
  completedDate: string | null;
  inspectorName: string;
  inspectorRole: string | null;
  status: string;
  overallRating: string | null;
  summary: string | null;
  checklistResponsesJson: string | null;
  weatherConditions: string | null;
  personnelPresent: number | null;
  signedOffBy: string | null;
  signedOffAt: string | null;
  findingCount: number;
  createdAt: string;
}

export interface InspectionFinding {
  id: string;
  inspectionId: string;
  inspectionTitle: string;
  findingNumber: string;
  category: string;
  severity: string;
  description: string;
  location: string | null;
  recommendedAction: string | null;
  assignedTo: string | null;
  actionDueDate: string | null;
  actionCompletedDate: string | null;
  status: string;
  closureNotes: string | null;
  createdAt: string;
}

// Equipment & Maintenance (CMMS)
export interface EquipmentItem {
  id: string;
  mineSiteId: string;
  mineSiteName: string;
  mineAreaId: string | null;
  mineAreaName: string | null;
  assetNumber: string;
  name: string;
  category: string;
  make: string | null;
  model: string | null;
  serialNumber: string | null;
  yearOfManufacture: number | null;
  purchaseDate: string | null;
  purchaseCost: number | null;
  status: string;
  location: string | null;
  operatorName: string | null;
  hoursOperated: number | null;
  nextServiceHours: number | null;
  nextServiceDate: string | null;
  lastServiceDate: string | null;
  warrantyInfo: string | null;
  notes: string | null;
  maintenanceCount: number;
  createdAt: string;
}

export interface MaintenanceRecord {
  id: string;
  equipmentId: string;
  equipmentName: string;
  assetNumber: string;
  workOrderNumber: string;
  maintenanceType: string;
  priority: string;
  title: string;
  description: string | null;
  scheduledDate: string;
  startedAt: string | null;
  completedAt: string | null;
  performedBy: string | null;
  status: string;
  downtimeHours: number | null;
  laborCost: number | null;
  partsCost: number | null;
  partsUsed: string | null;
  findings: string | null;
  actionsTaken: string | null;
  notes: string | null;
  createdAt: string;
}

// Personnel & Workforce Management
export interface PersonnelRecord {
  id: string;
  mineSiteId: string;
  mineSiteName: string;
  employeeNumber: string;
  firstName: string;
  lastName: string;
  middleName: string | null;
  role: string;
  department: string | null;
  designation: string | null;
  employmentType: string;
  dateOfJoining: string;
  dateOfLeaving: string | null;
  status: string;
  contactPhone: string | null;
  contactEmail: string | null;
  emergencyContactName: string | null;
  emergencyContactPhone: string | null;
  bloodGroup: string | null;
  medicalFitnessCertificate: string | null;
  medicalFitnessExpiry: string | null;
  notes: string | null;
  certificationCount: number;
  createdAt: string;
}

// Blasting & Explosives Management
export interface BlastEvent {
  id: string;
  mineSiteId: string;
  mineSiteName: string;
  mineAreaId: string | null;
  mineAreaName: string | null;
  blastNumber: string;
  title: string;
  blastType: string;
  scheduledDateTime: string;
  actualDateTime: string | null;
  location: string;
  drillingPattern: string | null;
  numberOfHoles: number | null;
  totalExplosivesKg: number | null;
  explosiveType: string | null;
  detonatorType: string | null;
  status: string;
  blastDesignNotes: string | null;
  safetyRadius: number | null;
  evacuationConfirmed: boolean;
  sentryPostsConfirmed: boolean;
  preBlastWarningGiven: boolean;
  supervisorName: string;
  licensedBlasterName: string;
  vibrationReading: number | null;
  airBlastReading: number | null;
  postBlastInspection: string | null;
  postBlastNotes: string | null;
  fragmentationQuality: string | null;
  misfireCount: number;
  createdAt: string;
  usageCount: number;
}

export interface ExplosiveUsage {
  id: string;
  blastEventId: string;
  blastTitle: string;
  explosiveName: string;
  type: string;
  batchNumber: string | null;
  quantityIssued: number;
  quantityUsed: number;
  quantityReturned: number;
  unit: string;
  magazineSource: string | null;
  issuedBy: string | null;
  receivedBy: string | null;
  notes: string | null;
  createdAt: string;
}

export interface PersonnelCertification {
  id: string;
  personnelId: string;
  personnelName: string;
  certificationName: string;
  certificateNumber: string | null;
  issuingAuthority: string | null;
  issueDate: string;
  expiryDate: string | null;
  status: string;
  category: string | null;
  notes: string | null;
  createdAt: string;
}

// Production & Dispatch
export interface ProductionLog {
  id: string;
  mineSiteId: string;
  mineSiteName: string;
  mineAreaId: string | null;
  mineAreaName: string | null;
  shiftInstanceId: string | null;
  logNumber: string;
  date: string;
  shiftName: string | null;
  material: string;
  sourceLocation: string | null;
  destinationLocation: string | null;
  quantityTonnes: number;
  quantityBCM: number | null;
  equipmentUsed: string | null;
  operatorName: string | null;
  haulingDistance: number | null;
  loadCount: number | null;
  status: string;
  notes: string | null;
  verifiedBy: string | null;
  verifiedAt: string | null;
  createdAt: string;
}

export interface DispatchRecord {
  id: string;
  mineSiteId: string;
  mineSiteName: string;
  dispatchNumber: string;
  date: string;
  vehicleNumber: string;
  driverName: string | null;
  material: string;
  sourceLocation: string;
  destinationLocation: string;
  weighbridgeTicketNumber: string | null;
  grossWeight: number | null;
  tareWeight: number | null;
  netWeight: number | null;
  unit: string;
  departureTime: string | null;
  arrivalTime: string | null;
  status: string;
  notes: string | null;
  createdAt: string;
}

// Environmental Monitoring
export interface EnvironmentalReading {
  id: string;
  mineSiteId: string;
  mineSiteName: string;
  mineAreaId: string | null;
  mineAreaName: string | null;
  readingNumber: string;
  readingType: string;
  parameter: string;
  value: number;
  unit: string;
  thresholdMin: number | null;
  thresholdMax: number | null;
  isExceedance: boolean;
  readingDateTime: string;
  monitoringStation: string | null;
  instrumentUsed: string | null;
  calibratedDate: string | null;
  recordedBy: string;
  weatherConditions: string | null;
  notes: string | null;
  status: string;
  createdAt: string;
}

export interface EnvironmentalIncident {
  id: string;
  mineSiteId: string;
  mineSiteName: string;
  incidentNumber: string;
  title: string;
  incidentType: string;
  severity: string;
  occurredAt: string;
  location: string;
  description: string;
  impactAssessment: string | null;
  containmentActions: string | null;
  remediationPlan: string | null;
  reportedBy: string;
  notifiedAuthority: boolean;
  authorityReference: string | null;
  status: string;
  closedAt: string | null;
  closureNotes: string | null;
  createdAt: string;
}

export interface WorkPermit {
  id: string;
  mineSiteId: string;
  mineSiteName: string;
  mineAreaId: string | null;
  mineAreaName: string | null;
  permitNumber: string;
  title: string;
  permitType: string;
  requestedBy: string;
  requestDate: string;
  startDateTime: string;
  endDateTime: string;
  location: string;
  workDescription: string;
  hazardsIdentified: string | null;
  controlMeasures: string | null;
  ppeRequired: string | null;
  emergencyProcedures: string | null;
  gasTestRequired: boolean;
  gasTestResults: string | null;
  status: string;
  approvedBy: string | null;
  approvedAt: string | null;
  closedBy: string | null;
  closedAt: string | null;
  rejectionReason: string | null;
  notes: string | null;
  createdAt: string;
}
