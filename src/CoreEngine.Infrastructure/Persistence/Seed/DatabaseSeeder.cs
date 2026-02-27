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

        // Admin role - User, Role, Department, AuditLog.Read, Tenant.Read + Phase 2 read permissions
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
            Permissions.Tenants.Read,
            // Phase 2
            Permissions.Configuration.Read,
            Permissions.Configuration.Update,
            Permissions.FeatureFlags.Read,
            Permissions.FeatureFlags.Update,
            Permissions.Notifications.Read,
            Permissions.Notifications.Send,
            Permissions.StateMachine.Read,
            Permissions.StateMachine.Manage,
            Permissions.StateMachine.Transition,
            Permissions.BackgroundJobs.Read,
            Permissions.BackgroundJobs.Manage,
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

        // 5. Seed Phase 2 defaults
        await SeedPhase2Defaults(context);
    }

    private static async Task SeedPhase2Defaults(ApplicationDbContext context)
    {
        // Default system configurations
        var existingConfigs = await context.SystemConfigurations
            .IgnoreQueryFilters()
            .Where(c => c.TenantId == DefaultTenantId)
            .ToListAsync();

        if (!existingConfigs.Any())
        {
            var defaultConfigs = new[]
            {
                new SystemConfiguration { Key = "App.Name", Value = "CoreEngine", Category = "General", Description = "Application display name", DataType = "String", TenantId = DefaultTenantId },
                new SystemConfiguration { Key = "App.PageSize", Value = "25", Category = "General", Description = "Default page size for lists", DataType = "Int", TenantId = DefaultTenantId },
                new SystemConfiguration { Key = "Email.FromAddress", Value = "noreply@coreengine.local", Category = "Email", Description = "Default sender email", DataType = "String", TenantId = DefaultTenantId },
                new SystemConfiguration { Key = "Email.FromName", Value = "CoreEngine", Category = "Email", Description = "Default sender name", DataType = "String", TenantId = DefaultTenantId },
                new SystemConfiguration { Key = "Session.TimeoutMinutes", Value = "60", Category = "Security", Description = "Session timeout in minutes", DataType = "Int", TenantId = DefaultTenantId },
                new SystemConfiguration { Key = "Password.MinLength", Value = "8", Category = "Security", Description = "Minimum password length", DataType = "Int", TenantId = DefaultTenantId },
            };

            foreach (var cfg in defaultConfigs)
            {
                cfg.CreatedAt = DateTime.UtcNow;
                cfg.CreatedBy = AppConstants.SystemUser;
            }

            context.SystemConfigurations.AddRange(defaultConfigs);
            await context.SaveChangesAsync();
        }

        // Default feature flags
        var existingFlags = await context.FeatureFlags
            .IgnoreQueryFilters()
            .Where(f => f.TenantId == DefaultTenantId)
            .ToListAsync();

        if (!existingFlags.Any())
        {
            var defaultFlags = new[]
            {
                new FeatureFlag { Name = "Notifications.Email", Description = "Enable email notifications", IsEnabled = false, TenantId = DefaultTenantId },
                new FeatureFlag { Name = "Notifications.InApp", Description = "Enable in-app notifications", IsEnabled = true, TenantId = DefaultTenantId },
                new FeatureFlag { Name = "AuditLog.DetailedDiff", Description = "Show detailed field-level diffs in audit logs", IsEnabled = true, TenantId = DefaultTenantId },
                new FeatureFlag { Name = "StateMachine.Enabled", Description = "Enable state machine transitions", IsEnabled = true, TenantId = DefaultTenantId },
                new FeatureFlag { Name = "BackgroundJobs.Enabled", Description = "Enable background job processing", IsEnabled = true, TenantId = DefaultTenantId },
            };

            foreach (var flag in defaultFlags)
            {
                flag.CreatedAt = DateTime.UtcNow;
                flag.CreatedBy = AppConstants.SystemUser;
            }

            context.FeatureFlags.AddRange(defaultFlags);
            await context.SaveChangesAsync();
        }

        // Sample state definitions (Task entity as example)
        var existingStates = await context.StateDefinitions
            .IgnoreQueryFilters()
            .Where(s => s.TenantId == DefaultTenantId && s.EntityType == "Task")
            .ToListAsync();

        if (!existingStates.Any())
        {
            var states = new[]
            {
                new StateDefinition { EntityType = "Task", StateName = "Draft", IsInitial = true, IsFinal = false, Color = "#86868b", SortOrder = 0, TenantId = DefaultTenantId },
                new StateDefinition { EntityType = "Task", StateName = "Open", IsInitial = false, IsFinal = false, Color = "#0071e3", SortOrder = 1, TenantId = DefaultTenantId },
                new StateDefinition { EntityType = "Task", StateName = "InProgress", IsInitial = false, IsFinal = false, Color = "#ff9500", SortOrder = 2, TenantId = DefaultTenantId },
                new StateDefinition { EntityType = "Task", StateName = "Review", IsInitial = false, IsFinal = false, Color = "#af52de", SortOrder = 3, TenantId = DefaultTenantId },
                new StateDefinition { EntityType = "Task", StateName = "Done", IsInitial = false, IsFinal = true, Color = "#34c759", SortOrder = 4, TenantId = DefaultTenantId },
                new StateDefinition { EntityType = "Task", StateName = "Cancelled", IsInitial = false, IsFinal = true, Color = "#ff3b30", SortOrder = 5, TenantId = DefaultTenantId },
            };

            foreach (var s in states)
            {
                s.CreatedAt = DateTime.UtcNow;
                s.CreatedBy = AppConstants.SystemUser;
            }

            context.StateDefinitions.AddRange(states);
            await context.SaveChangesAsync();

            var transitions = new[]
            {
                new StateTransitionDefinition { EntityType = "Task", FromState = "Draft", ToState = "Open", TriggerName = "Submit", Description = "Submit draft for work", TenantId = DefaultTenantId },
                new StateTransitionDefinition { EntityType = "Task", FromState = "Open", ToState = "InProgress", TriggerName = "Start", Description = "Begin working", TenantId = DefaultTenantId },
                new StateTransitionDefinition { EntityType = "Task", FromState = "InProgress", ToState = "Review", TriggerName = "RequestReview", Description = "Submit for review", TenantId = DefaultTenantId },
                new StateTransitionDefinition { EntityType = "Task", FromState = "Review", ToState = "InProgress", TriggerName = "Rework", Description = "Send back for rework", TenantId = DefaultTenantId },
                new StateTransitionDefinition { EntityType = "Task", FromState = "Review", ToState = "Done", TriggerName = "Approve", Description = "Approve and complete", TenantId = DefaultTenantId },
                new StateTransitionDefinition { EntityType = "Task", FromState = "Open", ToState = "Cancelled", TriggerName = "Cancel", Description = "Cancel task", TenantId = DefaultTenantId },
                new StateTransitionDefinition { EntityType = "Task", FromState = "InProgress", ToState = "Cancelled", TriggerName = "Cancel", Description = "Cancel task", TenantId = DefaultTenantId },
            };

            foreach (var t in transitions)
            {
                t.CreatedAt = DateTime.UtcNow;
                t.CreatedBy = AppConstants.SystemUser;
            }

            context.StateTransitionDefinitions.AddRange(transitions);
            await context.SaveChangesAsync();
        }
    }
}
