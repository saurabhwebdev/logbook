using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class Role : TenantScopedEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }

    // Navigation
    public Tenant Tenant { get; set; } = default!;
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
