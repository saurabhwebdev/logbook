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
