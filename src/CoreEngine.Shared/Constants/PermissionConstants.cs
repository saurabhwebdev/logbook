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
