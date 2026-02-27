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

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
