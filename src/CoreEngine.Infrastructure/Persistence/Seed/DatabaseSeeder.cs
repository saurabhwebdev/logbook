using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Enums;
using CoreEngine.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    private static readonly Guid DefaultTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        // 1. Seed permissions
        var allPermissionStrings = Permissions.GetAll();
        var existingPermissions = await context.Permissions
            .IgnoreQueryFilters()
            .ToListAsync();

        var newPermissions = new List<Permission>();
        foreach (var permStr in allPermissionStrings)
        {
            var parts = permStr.Split('.');
            if (parts.Length != 2)
                continue;

            var module = parts[0];
            var action = parts[1];

            var exists = existingPermissions.Any(p => p.Module == module && p.Action == action);
            if (!exists)
            {
                var permission = new Permission
                {
                    Id = Guid.NewGuid(),
                    Module = module,
                    Action = action,
                    Description = $"{action} {module}",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = AppConstants.SystemUser
                };
                newPermissions.Add(permission);
                context.Permissions.Add(permission);
            }
        }

        if (newPermissions.Count > 0)
        {
            await context.SaveChangesAsync();
        }

        // Reload all permissions after potential insert
        var allPermissions = await context.Permissions
            .IgnoreQueryFilters()
            .ToListAsync();

        // 2. Seed default tenant
        var defaultTenant = await context.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == DefaultTenantId);

        if (defaultTenant is null)
        {
            defaultTenant = new Tenant
            {
                Id = DefaultTenantId,
                Name = AppConstants.DefaultTenantName,
                Subdomain = AppConstants.DefaultTenantSubdomain,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = AppConstants.SystemUser
            };
            context.Tenants.Add(defaultTenant);
            await context.SaveChangesAsync();
        }

        // 3. Seed roles for default tenant
        var existingRoles = await context.Roles
            .IgnoreQueryFilters()
            .Where(r => r.TenantId == DefaultTenantId)
            .Include(r => r.RolePermissions)
            .ToListAsync();

        // SuperAdmin role - all permissions
        var superAdminRole = existingRoles.FirstOrDefault(r => r.Name == RoleConstants.SuperAdmin);
        if (superAdminRole is null)
        {
            superAdminRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = RoleConstants.SuperAdmin,
                Description = "Full system access",
                IsSystemRole = true,
                TenantId = DefaultTenantId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = AppConstants.SystemUser
            };
            context.Roles.Add(superAdminRole);
        }

        // Assign all permissions to SuperAdmin
        foreach (var perm in allPermissions)
        {
            if (!superAdminRole.RolePermissions.Any(rp => rp.PermissionId == perm.Id))
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = superAdminRole.Id,
                    PermissionId = perm.Id
                });
            }
        }

        // Admin role - User, Role, Department, AuditLog.Read, Tenant.Read permissions
        var adminPermissionFilter = new HashSet<string>
        {
            Permissions.Users.Create,
            Permissions.Users.Read,
            Permissions.Users.Update,
            Permissions.Users.Delete,
            Permissions.Roles.Create,
            Permissions.Roles.Read,
            Permissions.Roles.Update,
            Permissions.Roles.Delete,
            Permissions.Departments.Create,
            Permissions.Departments.Read,
            Permissions.Departments.Update,
            Permissions.Departments.Delete,
            Permissions.AuditLogs.Read,
            Permissions.Tenants.Read
        };

        var adminRole = existingRoles.FirstOrDefault(r => r.Name == RoleConstants.Admin);
        if (adminRole is null)
        {
            adminRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = RoleConstants.Admin,
                Description = "Administrative access",
                IsSystemRole = true,
                TenantId = DefaultTenantId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = AppConstants.SystemUser
            };
            context.Roles.Add(adminRole);
        }

        var adminPermissions = allPermissions.Where(p => adminPermissionFilter.Contains(p.FullPermission));
        foreach (var perm in adminPermissions)
        {
            if (!adminRole.RolePermissions.Any(rp => rp.PermissionId == perm.Id))
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = perm.Id
                });
            }
        }

        // User role - only User.Read and Department.Read
        var userPermissionFilter = new HashSet<string>
        {
            Permissions.Users.Read,
            Permissions.Departments.Read
        };

        var userRole = existingRoles.FirstOrDefault(r => r.Name == RoleConstants.User);
        if (userRole is null)
        {
            userRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = RoleConstants.User,
                Description = "Standard user access",
                IsSystemRole = true,
                TenantId = DefaultTenantId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = AppConstants.SystemUser
            };
            context.Roles.Add(userRole);
        }

        var userPermissions = allPermissions.Where(p => userPermissionFilter.Contains(p.FullPermission));
        foreach (var perm in userPermissions)
        {
            if (!userRole.RolePermissions.Any(rp => rp.PermissionId == perm.Id))
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = userRole.Id,
                    PermissionId = perm.Id
                });
            }
        }

        await context.SaveChangesAsync();

        // 4. Seed admin user
        var adminUserExists = await context.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Email == "admin@coreengine.local");

        if (!adminUserExists)
        {
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@coreengine.local",
                PasswordHash = passwordHasher.Hash("Admin@123"),
                FirstName = "System",
                LastName = "Administrator",
                TenantId = DefaultTenantId,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = AppConstants.SystemUser
            };

            context.Users.Add(adminUser);

            context.UserRoles.Add(new UserRole
            {
                UserId = adminUser.Id,
                RoleId = superAdminRole.Id
            });

            await context.SaveChangesAsync();
        }
    }
}
