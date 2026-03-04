using System.Reflection;

namespace CoreEngine.Shared.Constants;

public static class Permissions
{
    public static class Users
    {
        public const string Create = "User.Create";
        public const string Read = "User.Read";
        public const string Update = "User.Update";
        public const string Delete = "User.Delete";
    }

    public static class Roles
    {
        public const string Create = "Role.Create";
        public const string Read = "Role.Read";
        public const string Update = "Role.Update";
        public const string Delete = "Role.Delete";
    }

    public static class Departments
    {
        public const string Create = "Department.Create";
        public const string Read = "Department.Read";
        public const string Update = "Department.Update";
        public const string Delete = "Department.Delete";
    }

    public static class AuditLogs
    {
        public const string Read = "AuditLog.Read";
    }

    public static class Tenants
    {
        public const string Create = "Tenant.Create";
        public const string Read = "Tenant.Read";
        public const string Update = "Tenant.Update";
    }

    public static class Configuration
    {
        public const string Read = "Configuration.Read";
        public const string Update = "Configuration.Update";
    }

    public static class FeatureFlags
    {
        public const string Create = "FeatureFlag.Create";
        public const string Read = "FeatureFlag.Read";
        public const string Update = "FeatureFlag.Update";
        public const string Delete = "FeatureFlag.Delete";
    }

    public static class Notifications
    {
        public const string Read = "Notification.Read";
        public const string Send = "Notification.Send";
    }

    public static class StateMachine
    {
        public const string Read = "StateMachine.Read";
        public const string Manage = "StateMachine.Manage";
        public const string Transition = "StateMachine.Transition";
    }

    public static class BackgroundJobs
    {
        public const string Read = "BackgroundJob.Read";
        public const string Manage = "BackgroundJob.Manage";
    }

    // Phase 3
    public static class Files
    {
        public const string Upload = "File.Upload";
        public const string Read = "File.Read";
        public const string Delete = "File.Delete";
    }

    public static class Reports
    {
        public const string Create = "Report.Create";
        public const string Read = "Report.Read";
        public const string Export = "Report.Export";
        public const string Delete = "Report.Delete";
    }

    public static class ApiIntegration
    {
        public const string Read = "ApiIntegration.Read";
        public const string Manage = "ApiIntegration.Manage";
    }

    public static class DemoTasks
    {
        public const string Create = "DemoTask.Create";
        public const string Read = "DemoTask.Read";
        public const string Update = "DemoTask.Update";
        public const string Delete = "DemoTask.Delete";
        public const string Transition = "DemoTask.Transition";
    }

    // Help Module
    public static class Help
    {
        public const string Create = "Help.Create";
        public const string Read = "Help.Read";
        public const string Update = "Help.Update";
        public const string Delete = "Help.Delete";
    }

    // Email Module
    public static class EmailTemplates
    {
        public const string Create = "EmailTemplate.Create";
        public const string Read = "EmailTemplate.Read";
        public const string Update = "EmailTemplate.Update";
        public const string Delete = "EmailTemplate.Delete";
    }

    public static class Emails
    {
        public const string Read = "Email.Read";
        public const string Send = "Email.Send";
    }

    // ===== Logbook Mining Modules =====

    // Mine Site Management
    public static class MineSites
    {
        public const string Create = "MineSite.Create";
        public const string Read = "MineSite.Read";
        public const string Update = "MineSite.Update";
        public const string Delete = "MineSite.Delete";
    }

    public static class MineAreas
    {
        public const string Create = "MineArea.Create";
        public const string Read = "MineArea.Read";
        public const string Update = "MineArea.Update";
        public const string Delete = "MineArea.Delete";
    }

    // Shift Management & Handover
    public static class ShiftDefinitions
    {
        public const string Create = "ShiftDefinition.Create";
        public const string Read = "ShiftDefinition.Read";
        public const string Update = "ShiftDefinition.Update";
        public const string Delete = "ShiftDefinition.Delete";
    }

    public static class ShiftInstances
    {
        public const string Create = "ShiftInstance.Create";
        public const string Read = "ShiftInstance.Read";
        public const string Update = "ShiftInstance.Update";
        public const string Delete = "ShiftInstance.Delete";
    }

    public static class ShiftHandovers
    {
        public const string Create = "ShiftHandover.Create";
        public const string Read = "ShiftHandover.Read";
        public const string Update = "ShiftHandover.Update";
        public const string Delete = "ShiftHandover.Delete";
    }

    // Statutory Registers
    public static class StatutoryRegisters
    {
        public const string Create = "StatutoryRegister.Create";
        public const string Read = "StatutoryRegister.Read";
        public const string Update = "StatutoryRegister.Update";
        public const string Delete = "StatutoryRegister.Delete";
    }

    public static class RegisterEntries
    {
        public const string Create = "RegisterEntry.Create";
        public const string Read = "RegisterEntry.Read";
        public const string Amend = "RegisterEntry.Amend";
    }

    // Safety & Incident Management
    public static class SafetyIncidents
    {
        public const string Create = "SafetyIncident.Create";
        public const string Read = "SafetyIncident.Read";
        public const string Update = "SafetyIncident.Update";
        public const string Delete = "SafetyIncident.Delete";
        public const string Investigate = "SafetyIncident.Investigate";
    }

    // Inspection & Audit Management
    public static class Inspections
    {
        public const string Create = "Inspection.Create";
        public const string Read = "Inspection.Read";
        public const string Update = "Inspection.Update";
        public const string Delete = "Inspection.Delete";
        public const string ManageTemplates = "Inspection.ManageTemplates";
    }

    // Equipment & Maintenance (CMMS)
    public static class EquipmentMgmt
    {
        public const string Create = "Equipment.Create";
        public const string Read = "Equipment.Read";
        public const string Update = "Equipment.Update";
        public const string Delete = "Equipment.Delete";
        public const string Maintain = "Equipment.Maintain";
    }

    // Blasting & Explosives Management
    public static class Blasting
    {
        public const string Create = "Blasting.Create";
        public const string Read = "Blasting.Read";
        public const string Update = "Blasting.Update";
        public const string Delete = "Blasting.Delete";
        public const string ManageExplosives = "Blasting.ManageExplosives";
    }

    // Production & Dispatch
    public static class Production
    {
        public const string Create = "Production.Create";
        public const string Read = "Production.Read";
        public const string Update = "Production.Update";
        public const string Delete = "Production.Delete";
    }

    public static class Dispatch
    {
        public const string Create = "Dispatch.Create";
        public const string Read = "Dispatch.Read";
        public const string Update = "Dispatch.Update";
        public const string Delete = "Dispatch.Delete";
    }

    // Permit to Work
    public static class WorkPermits
    {
        public const string Create = "WorkPermit.Create";
        public const string Read = "WorkPermit.Read";
        public const string Update = "WorkPermit.Update";
        public const string Delete = "WorkPermit.Delete";
        public const string Approve = "WorkPermit.Approve";
    }

    // Environmental Monitoring
    public static class Environmental
    {
        public const string Create = "Environmental.Create";
        public const string Read = "Environmental.Read";
        public const string Update = "Environmental.Update";
        public const string Delete = "Environmental.Delete";
        public const string ManageIncidents = "Environmental.ManageIncidents";
    }

    // Personnel & Workforce Management
    public static class PersonnelMgmt
    {
        public const string Create = "Personnel.Create";
        public const string Read = "Personnel.Read";
        public const string Update = "Personnel.Update";
        public const string Delete = "Personnel.Delete";
        public const string ManageCertifications = "Personnel.ManageCertifications";
    }

    // Workflow Engine
    public static class WorkflowDefinitions
    {
        public const string Create = "WorkflowDefinition.Create";
        public const string Read = "WorkflowDefinition.Read";
        public const string Update = "WorkflowDefinition.Update";
        public const string Delete = "WorkflowDefinition.Delete";
    }

    public static class Workflows
    {
        public const string Start = "Workflow.Start";
        public const string View = "Workflow.View";
        public const string Cancel = "Workflow.Cancel";
    }

    public static class WorkflowTasks
    {
        public const string View = "WorkflowTask.View";
        public const string Complete = "WorkflowTask.Complete";
        public const string Reassign = "WorkflowTask.Reassign";
    }

    /// <summary>
    /// Collects all permission strings via reflection for seeding.
    /// </summary>
    public static IReadOnlyList<string> GetAll()
    {
        var permissions = new List<string>();

        var nestedTypes = typeof(Permissions).GetNestedTypes(BindingFlags.Public | BindingFlags.Static);
        foreach (var type in nestedTypes)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

            foreach (var field in fields)
            {
                var value = field.GetRawConstantValue() as string;
                if (value is not null)
                    permissions.Add(value);
            }
        }

        return permissions.AsReadOnly();
    }
}
