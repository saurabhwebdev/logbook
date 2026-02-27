namespace CoreEngine.Application.Features.Roles.Queries.GetRoles;

public record RoleDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsSystemRole,
    DateTime CreatedAt,
    int PermissionCount,
    IReadOnlyList<string> Permissions
);
