using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

/// <summary>
/// Global permission catalog. Not tenant-scoped.
/// Format: Module.Action (e.g., "User.Create")
/// </summary>
public class Permission : BaseEntity
{
    public string Module { get; set; } = default!;
    public string Action { get; set; } = default!;
    public string? Description { get; set; }

    // Navigation
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    /// <summary>
    /// Returns "Module.Action" format
    /// </summary>
    public string FullPermission => $"{Module}.{Action}";
}
