using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<Department> Departments { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<AuditLog> AuditLogs { get; }

    // Phase 2
    DbSet<SystemConfiguration> SystemConfigurations { get; }
    DbSet<FeatureFlag> FeatureFlags { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<StateDefinition> StateDefinitions { get; }
    DbSet<StateTransitionDefinition> StateTransitionDefinitions { get; }
    DbSet<StateTransitionLog> StateTransitionLogs { get; }

    // Phase 3
    DbSet<FileMetadata> FileMetadata { get; }
    DbSet<ReportDefinition> ReportDefinitions { get; }
    DbSet<ApiKey> ApiKeys { get; }
    DbSet<WebhookSubscription> WebhookSubscriptions { get; }
    DbSet<DemoTask> DemoTasks { get; }

    // Help Module
    DbSet<HelpArticle> HelpArticles { get; }

    // Email Module
    DbSet<EmailTemplate> EmailTemplates { get; }
    DbSet<EmailQueue> EmailQueues { get; }

    // Workflow Engine
    DbSet<WorkflowDefinition> WorkflowDefinitions { get; }
    DbSet<WorkflowInstance> WorkflowInstances { get; }
    DbSet<WorkflowTask> WorkflowTasks { get; }

    // Logbook Mining Modules
    DbSet<MineSite> MineSites { get; }
    DbSet<MineArea> MineAreas { get; }

    // Shift Management & Handover
    DbSet<ShiftDefinition> ShiftDefinitions { get; }
    DbSet<ShiftInstance> ShiftInstances { get; }
    DbSet<ShiftHandover> ShiftHandovers { get; }

    // Statutory Registers
    DbSet<StatutoryRegister> StatutoryRegisters { get; }
    DbSet<RegisterEntry> RegisterEntries { get; }

    // Safety & Incident Management
    DbSet<SafetyIncident> SafetyIncidents { get; }
    DbSet<IncidentInvestigation> IncidentInvestigations { get; }

    // Inspection & Audit Management
    DbSet<InspectionTemplate> InspectionTemplates { get; }
    DbSet<Inspection> Inspections { get; }
    DbSet<InspectionFinding> InspectionFindings { get; }

    // Equipment & Maintenance (CMMS)
    DbSet<Equipment> Equipment { get; }
    DbSet<MaintenanceRecord> MaintenanceRecords { get; }

    // Personnel & Workforce Management
    DbSet<Personnel> Personnel { get; }
    DbSet<PersonnelCertification> PersonnelCertifications { get; }

    // Blasting & Explosives Management
    DbSet<BlastEvent> BlastEvents { get; }
    DbSet<ExplosiveUsage> ExplosiveUsages { get; }

    // Production & Dispatch
    DbSet<ProductionLog> ProductionLogs { get; }
    DbSet<DispatchRecord> DispatchRecords { get; }

    // Permit to Work
    DbSet<WorkPermit> WorkPermits { get; }

    // Environmental Monitoring
    DbSet<EnvironmentalReading> EnvironmentalReadings { get; }
    DbSet<EnvironmentalIncident> EnvironmentalIncidents { get; }

    // Ventilation & Gas Monitoring
    DbSet<VentilationReading> VentilationReadings { get; }
    DbSet<GasReading> GasReadings { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
