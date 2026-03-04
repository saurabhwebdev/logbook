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
            // Phase 3
            Permissions.Files.Upload,
            Permissions.Files.Read,
            Permissions.Files.Delete,
            Permissions.Reports.Create,
            Permissions.Reports.Read,
            Permissions.Reports.Export,
            Permissions.Reports.Delete,
            Permissions.ApiIntegration.Read,
            Permissions.ApiIntegration.Manage,
            Permissions.DemoTasks.Create,
            Permissions.DemoTasks.Read,
            Permissions.DemoTasks.Update,
            Permissions.DemoTasks.Delete,
            Permissions.DemoTasks.Transition,
            // Help Module
            Permissions.Help.Create,
            Permissions.Help.Read,
            Permissions.Help.Update,
            Permissions.Help.Delete,
            // Logbook Mining Modules
            Permissions.MineSites.Create,
            Permissions.MineSites.Read,
            Permissions.MineSites.Update,
            Permissions.MineSites.Delete,
            Permissions.MineAreas.Create,
            Permissions.MineAreas.Read,
            Permissions.MineAreas.Update,
            Permissions.MineAreas.Delete,
            // Shift Management & Handover
            Permissions.ShiftDefinitions.Create,
            Permissions.ShiftDefinitions.Read,
            Permissions.ShiftDefinitions.Update,
            Permissions.ShiftDefinitions.Delete,
            Permissions.ShiftInstances.Create,
            Permissions.ShiftInstances.Read,
            Permissions.ShiftInstances.Update,
            Permissions.ShiftInstances.Delete,
            Permissions.ShiftHandovers.Create,
            Permissions.ShiftHandovers.Read,
            Permissions.ShiftHandovers.Update,
            Permissions.ShiftHandovers.Delete,
            // Statutory Registers
            Permissions.StatutoryRegisters.Create,
            Permissions.StatutoryRegisters.Read,
            Permissions.StatutoryRegisters.Update,
            Permissions.StatutoryRegisters.Delete,
            Permissions.RegisterEntries.Create,
            Permissions.RegisterEntries.Read,
            Permissions.RegisterEntries.Amend,
            // Safety & Incident Management
            Permissions.SafetyIncidents.Create,
            Permissions.SafetyIncidents.Read,
            Permissions.SafetyIncidents.Update,
            Permissions.SafetyIncidents.Delete,
            Permissions.SafetyIncidents.Investigate,
            // Inspection & Audit Management
            Permissions.Inspections.Create,
            Permissions.Inspections.Read,
            Permissions.Inspections.Update,
            Permissions.Inspections.Delete,
            Permissions.Inspections.ManageTemplates,
            // Equipment & Maintenance (CMMS)
            Permissions.EquipmentMgmt.Create,
            Permissions.EquipmentMgmt.Read,
            Permissions.EquipmentMgmt.Update,
            Permissions.EquipmentMgmt.Delete,
            Permissions.EquipmentMgmt.Maintain,
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
            Permissions.Departments.Read,
            Permissions.Help.Read,
            Permissions.MineSites.Read,
            Permissions.MineAreas.Read,
            Permissions.ShiftDefinitions.Read,
            Permissions.ShiftInstances.Read,
            Permissions.ShiftHandovers.Read,
            Permissions.StatutoryRegisters.Read,
            Permissions.RegisterEntries.Read,
            Permissions.SafetyIncidents.Read,
            Permissions.Inspections.Read,
            Permissions.EquipmentMgmt.Read,
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

        // 6. Seed Email Templates
        await SeedEmailTemplates(context);

        // 7. Seed Help articles
        await SeedHelpArticles(context);

        // 8. Seed Workflow Definitions
        await SeedWorkflowDefinitionsAsync(context);
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
                new SystemConfiguration { Key = "Email.SmtpHost", Value = "smtp.gmail.com", Category = "Email", Description = "SMTP server hostname", DataType = "String", TenantId = DefaultTenantId },
                new SystemConfiguration { Key = "Email.SmtpPort", Value = "587", Category = "Email", Description = "SMTP server port", DataType = "Int", TenantId = DefaultTenantId },
                new SystemConfiguration { Key = "Email.SmtpUsername", Value = "", Category = "Email", Description = "SMTP username", DataType = "String", TenantId = DefaultTenantId },
                new SystemConfiguration { Key = "Email.SmtpPassword", Value = "", Category = "Email", Description = "SMTP password", DataType = "String", TenantId = DefaultTenantId },
                new SystemConfiguration { Key = "Email.SmtpEnableSsl", Value = "true", Category = "Email", Description = "Enable SSL for SMTP", DataType = "Bool", TenantId = DefaultTenantId },
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
                new FeatureFlag { Name = "Help.Enabled", Description = "Enable the help center and contextual help", IsEnabled = true, TenantId = DefaultTenantId },
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

    private static async Task SeedHelpArticles(ApplicationDbContext context)
    {
        var existing = await context.HelpArticles
            .IgnoreQueryFilters()
            .Where(h => h.TenantId == DefaultTenantId)
            .AnyAsync();

        if (existing) return;

        var articles = new[]
        {
            new HelpArticle
            {
                Title = "Dashboard Overview",
                Slug = "dashboard",
                ModuleKey = "Dashboard",
                Category = "Getting Started",
                SortOrder = 0,
                IsPublished = true,
                Tags = "dashboard,overview,stats",
                TenantId = DefaultTenantId,
                Content = @"# Dashboard

The Dashboard provides a real-time overview of your system.

## Stats Cards
- **Users** — Total registered users in your tenant
- **Roles** — Number of configured roles
- **Departments** — Active departments
- **Audit Events** — Total audit log entries
- **Files** — Uploaded files count
- **Reports** — Saved report definitions
- **Active Tasks** — Demo tasks not yet completed
- **Feature Flags** — Enabled feature flags
- **API Keys** — Active API keys

## Recent Activity
The bottom section shows the last 5 audit log entries — who did what and when."
            },
            new HelpArticle
            {
                Title = "Managing Users",
                Slug = "users",
                ModuleKey = "Users",
                Category = "Admin Guide",
                SortOrder = 1,
                IsPublished = true,
                Tags = "users,accounts,roles,permissions",
                TenantId = DefaultTenantId,
                Content = @"# Users

Manage user accounts for your tenant.

## Creating a User
1. Click **New User**
2. Fill in email, password, first/last name
3. Assign one or more roles
4. Optionally assign to a department

## Editing a User
Click the **Edit** button in the actions column. You can change name, status, roles, and department.

## User Status
- **Active** — Can log in and use the system
- **Inactive** — Account disabled, cannot log in

## Permissions
Users inherit permissions from their assigned roles. The permission format is `Module.Action` (e.g., `User.Create`, `Report.Read`)."
            },
            new HelpArticle
            {
                Title = "Roles & Permissions",
                Slug = "roles",
                ModuleKey = "Roles",
                Category = "Admin Guide",
                SortOrder = 2,
                IsPublished = true,
                Tags = "roles,permissions,rbac,security",
                TenantId = DefaultTenantId,
                Content = @"# Roles & Permissions

CoreEngine uses **Role-Based Access Control (RBAC)** with flat permissions.

## Default Roles
- **SuperAdmin** — All permissions, full system access
- **Admin** — Administrative access to most modules
- **User** — Basic read-only access

## Permission Format
Permissions follow the `Module.Action` pattern:
- `User.Create`, `User.Read`, `User.Update`, `User.Delete`
- `Report.Read`, `Report.Export`
- `Help.Create`, `Help.Read`, etc.

## Creating Custom Roles
1. Go to **Roles** page
2. Click **New Role**
3. Name the role and select permissions
4. Assign users to the role"
            },
            new HelpArticle
            {
                Title = "Departments",
                Slug = "departments",
                ModuleKey = "Departments",
                Category = "Admin Guide",
                SortOrder = 3,
                IsPublished = true,
                Tags = "departments,organization,structure",
                TenantId = DefaultTenantId,
                Content = @"# Departments

Organize users into departments for reporting and organizational structure.

## Creating a Department
1. Click **New Department**
2. Enter name and optional code
3. Optionally select a parent department for hierarchy

## Assigning Users
Users can be assigned to departments during user creation or editing."
            },
            new HelpArticle
            {
                Title = "Audit Logs",
                Slug = "audit-logs",
                ModuleKey = "AuditLogs",
                Category = "Admin Guide",
                SortOrder = 4,
                IsPublished = true,
                Tags = "audit,logs,tracking,compliance",
                TenantId = DefaultTenantId,
                Content = @"# Audit Logs

Every create, update, and delete operation is automatically logged.

## What's Tracked
- **Entity Name** — Which entity was modified (User, Role, etc.)
- **Action** — Create, Update, or Delete
- **Old/New Values** — Field-level diff showing what changed
- **User** — Who made the change
- **IP Address** — Where the request came from
- **Timestamp** — When it happened

## Filtering
Use the filters to narrow down by entity type, action, or date range. Audit logs are **immutable** — they cannot be deleted."
            },
            new HelpArticle
            {
                Title = "Feature Flags",
                Slug = "feature-flags",
                ModuleKey = "FeatureFlags",
                Category = "Admin Guide",
                SortOrder = 5,
                IsPublished = true,
                Tags = "feature flags,toggles,configuration",
                TenantId = DefaultTenantId,
                Content = @"# Feature Flags

Toggle features on or off per tenant without code deployment.

## Default Flags
- **Notifications.Email** — Email sending (off by default)
- **Notifications.InApp** — In-app notifications (on)
- **AuditLog.DetailedDiff** — Field-level audit diffs (on)
- **StateMachine.Enabled** — State transitions (on)
- **Help.Enabled** — Help center (on)

## Usage
Click the toggle switch to enable/disable a feature. Changes take effect immediately."
            },
            new HelpArticle
            {
                Title = "Settings & Configuration",
                Slug = "settings",
                ModuleKey = "Settings",
                Category = "Admin Guide",
                SortOrder = 6,
                IsPublished = true,
                Tags = "settings,configuration,system",
                TenantId = DefaultTenantId,
                Content = @"# Settings

Manage system configuration values per tenant.

## Categories
- **General** — App name, page size
- **Email** — Sender address and display name
- **Security** — Session timeout, password requirements

## Editing
Click the **Edit** button next to any setting to change its value. Settings are stored as key-value pairs with type information."
            },
            new HelpArticle
            {
                Title = "File Management",
                Slug = "files",
                ModuleKey = "Files",
                Category = "User Guide",
                SortOrder = 7,
                IsPublished = true,
                Tags = "files,upload,download,storage",
                TenantId = DefaultTenantId,
                Content = @"# File Management

Upload, download, and manage files with metadata.

## Uploading Files
1. Click **Upload File**
2. Select a file (max 50MB)
3. Add optional description and category
4. Click **Upload**

## Storage
Files are stored on local disk by default. The `IFileStorageService` abstraction allows switching to Azure Blob Storage or AWS S3.

## Downloading
Click the file name or download icon to download."
            },
            new HelpArticle
            {
                Title = "Reports",
                Slug = "reports",
                ModuleKey = "Reports",
                Category = "User Guide",
                SortOrder = 8,
                IsPublished = true,
                Tags = "reports,export,excel,csv",
                TenantId = DefaultTenantId,
                Content = @"# Reports

Define and export data reports.

## Creating a Report
1. Click **New Report**
2. Choose entity type (User, Department, Role, AuditLog, DemoTask)
3. Select export format (Excel or CSV)
4. Define columns as JSON array

## Exporting
Click the **Download** button to generate and download the report. Excel files use ClosedXML for professional formatting."
            },
            new HelpArticle
            {
                Title = "API Integration",
                Slug = "api-integration",
                ModuleKey = "ApiIntegration",
                Category = "Developer Guide",
                SortOrder = 9,
                IsPublished = true,
                Tags = "api,keys,webhooks,integration",
                TenantId = DefaultTenantId,
                Content = @"# API Integration

Manage API keys and webhook subscriptions for external integrations.

## API Keys
- Generate keys for external system access
- Keys are shown **once** at creation — store securely
- Set expiration dates and scopes
- Deactivate keys without deleting them

## Webhooks
- Subscribe to events (e.g., `user.created`, `task.completed`)
- Each webhook gets an **HMAC signing secret** for verification
- Track delivery failures and last triggered time"
            },
            new HelpArticle
            {
                Title = "State Machine & Tasks",
                Slug = "state-machine",
                ModuleKey = "StateMachine",
                Category = "Developer Guide",
                SortOrder = 10,
                IsPublished = true,
                Tags = "state machine,workflow,transitions,lifecycle",
                TenantId = DefaultTenantId,
                Content = @"# State Machine

Define entity lifecycles with states and transitions.

## How It Works
1. **States** define where an entity can be (Draft, Open, InProgress, etc.)
2. **Transitions** define allowed moves between states
3. **Triggers** are named actions (Submit, Start, Approve, Cancel)

## Demo Tasks
The Tasks (Demo) page shows a working example:
- **Draft** → Submit → **Open** → Start → **InProgress** → Request Review → **Review** → Approve → **Done**
- Tasks can be **Cancelled** from Open or InProgress states

## Adding Your Own
Define new entity types with states and transitions in the State Machine page. Then use the transition API from your domain module."
            },
            new HelpArticle
            {
                Title = "Theming & Branding",
                Slug = "theming",
                ModuleKey = "Theming",
                Category = "Admin Guide",
                SortOrder = 11,
                IsPublished = true,
                Tags = "theming,branding,colors,logo",
                TenantId = DefaultTenantId,
                Content = @"# Theming

Customize your tenant's visual branding.

## Options
- **Logo URL** — URL to your organization's logo (displayed in sidebar)
- **Primary Color** — Affects buttons and accent colors
- **Sidebar Color** — Background color of the navigation sidebar

## How to Use
1. Go to the **Theming** page
2. Set your logo URL, primary color, and sidebar color
3. Click **Save Changes**
4. Changes apply immediately across the application"
            },
            new HelpArticle
            {
                Title = "Multi-Tenancy",
                Slug = "tenants",
                ModuleKey = "Tenants",
                Category = "Admin Guide",
                SortOrder = 12,
                IsPublished = true,
                Tags = "tenants,multi-tenancy,isolation",
                TenantId = DefaultTenantId,
                Content = @"# Multi-Tenancy

CoreEngine supports full data isolation between tenants.

## How It Works
- Every entity inherits from `TenantScopedEntity` which adds a `TenantId`
- EF Core **global query filters** automatically filter data by tenant
- The `TenantResolutionMiddleware` resolves the current tenant from JWT claims, headers, or subdomain

## Tenant Resolution Order
1. **JWT Claim** — `tenantId` claim from the access token
2. **X-Tenant-Id Header** — Explicit header for API calls
3. **Subdomain** — `tenant1.yourdomain.com`
4. **Default** — Falls back to the default tenant

## Creating Tenants
SuperAdmin users can create new tenants from the Tenants page."
            },
        };

        foreach (var article in articles)
        {
            article.CreatedAt = DateTime.UtcNow;
            article.CreatedBy = AppConstants.SystemUser;
        }

        context.HelpArticles.AddRange(articles);
        await context.SaveChangesAsync();
    }

    private static async Task SeedEmailTemplates(ApplicationDbContext context)
    {
        var existingTemplates = await context.EmailTemplates
            .IgnoreQueryFilters()
            .Where(t => t.TenantId == DefaultTenantId)
            .ToListAsync();

        if (existingTemplates.Any())
            return;

        var templates = new[]
        {
            new EmailTemplate
            {
                Name = "WelcomeEmail",
                Subject = "Welcome to {{appName}}",
                HtmlBody = @"<html>
<body style='font-family: Arial, sans-serif; line-height: 1.6;'>
    <h2>Welcome, {{userName}}!</h2>
    <p>Thank you for joining <strong>{{appName}}</strong>. We're excited to have you on board.</p>
    <p>Your account has been successfully created with the email: <strong>{{email}}</strong></p>
    <p>Click the link below to get started:</p>
    <p><a href='{{link}}' style='background-color: #0071e3; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Get Started</a></p>
    <br/>
    <p>Best regards,<br/>The {{appName}} Team</p>
</body>
</html>",
                PlainTextBody = "Welcome, {{userName}}!\n\nThank you for joining {{appName}}. Your account has been created with email: {{email}}.\n\nGet started here: {{link}}\n\nBest regards,\nThe {{appName}} Team",
                IsActive = true,
                TenantId = DefaultTenantId
            },
            new EmailTemplate
            {
                Name = "PasswordReset",
                Subject = "Reset Your Password - {{appName}}",
                HtmlBody = @"<html>
<body style='font-family: Arial, sans-serif; line-height: 1.6;'>
    <h2>Password Reset Request</h2>
    <p>Hi {{userName}},</p>
    <p>We received a request to reset your password for your {{appName}} account.</p>
    <p>Click the link below to reset your password:</p>
    <p><a href='{{link}}' style='background-color: #0071e3; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Reset Password</a></p>
    <p>This link will expire in 1 hour.</p>
    <p>If you didn't request a password reset, please ignore this email.</p>
    <br/>
    <p>Best regards,<br/>The {{appName}} Team</p>
</body>
</html>",
                PlainTextBody = "Hi {{userName}},\n\nWe received a request to reset your password.\n\nReset your password here: {{link}}\n\nThis link expires in 1 hour.\n\nBest regards,\nThe {{appName}} Team",
                IsActive = true,
                TenantId = DefaultTenantId
            },
            new EmailTemplate
            {
                Name = "TaskAssignment",
                Subject = "New Task Assigned: {{taskTitle}}",
                HtmlBody = @"<html>
<body style='font-family: Arial, sans-serif; line-height: 1.6;'>
    <h2>You've Been Assigned a New Task</h2>
    <p>Hi {{userName}},</p>
    <p>A new task has been assigned to you:</p>
    <div style='background-color: #f5f5f7; padding: 15px; border-radius: 8px; margin: 15px 0;'>
        <h3 style='margin-top: 0;'>{{taskTitle}}</h3>
        <p><strong>Assigned by:</strong> {{assignedBy}}</p>
        <p><strong>Due Date:</strong> {{dueDate}}</p>
    </div>
    <p>Click below to view the task details:</p>
    <p><a href='{{link}}' style='background-color: #0071e3; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>View Task</a></p>
    <br/>
    <p>Best regards,<br/>The {{appName}} Team</p>
</body>
</html>",
                PlainTextBody = "Hi {{userName}},\n\nA new task has been assigned to you:\n\nTask: {{taskTitle}}\nAssigned by: {{assignedBy}}\nDue Date: {{dueDate}}\n\nView task: {{link}}\n\nBest regards,\nThe {{appName}} Team",
                IsActive = true,
                TenantId = DefaultTenantId
            },
            new EmailTemplate
            {
                Name = "ApprovalRequest",
                Subject = "Approval Required: {{requestTitle}}",
                HtmlBody = @"<html>
<body style='font-family: Arial, sans-serif; line-height: 1.6;'>
    <h2>Approval Request</h2>
    <p>Hi {{userName}},</p>
    <p>Your approval is required for the following request:</p>
    <div style='background-color: #f5f5f7; padding: 15px; border-radius: 8px; margin: 15px 0;'>
        <h3 style='margin-top: 0;'>{{requestTitle}}</h3>
        <p><strong>Submitted by:</strong> {{submittedBy}}</p>
        <p><strong>Submitted on:</strong> {{date}}</p>
    </div>
    <p>Please review and take action:</p>
    <p><a href='{{link}}' style='background-color: #34c759; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; margin-right: 10px;'>Approve</a>
       <a href='{{rejectLink}}' style='background-color: #ff3b30; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Reject</a></p>
    <br/>
    <p>Best regards,<br/>The {{appName}} Team</p>
</body>
</html>",
                PlainTextBody = "Hi {{userName}},\n\nYour approval is required:\n\nRequest: {{requestTitle}}\nSubmitted by: {{submittedBy}}\nDate: {{date}}\n\nReview here: {{link}}\n\nBest regards,\nThe {{appName}} Team",
                IsActive = true,
                TenantId = DefaultTenantId
            }
        };

        foreach (var template in templates)
        {
            template.CreatedAt = DateTime.UtcNow;
            template.CreatedBy = AppConstants.SystemUser;
        }

        context.EmailTemplates.AddRange(templates);
        await context.SaveChangesAsync();
    }

    // 11. Seed Workflow Definitions
    private static async Task SeedWorkflowDefinitionsAsync(ApplicationDbContext context)
    {
        var existingDefinitions = await context.WorkflowDefinitions
            .IgnoreQueryFilters()
            .Where(d => d.TenantId == DefaultTenantId)
            .ToListAsync();

        if (existingDefinitions.Any())
            return;

        var definitions = new[]
        {
            new WorkflowDefinition
            {
                Name = "Purchase Order Approval",
                Description = "Multi-level approval workflow for purchase orders",
                Category = "Approval",
                ConfigurationJson = @"{
  ""Steps"": [
    { ""Name"": ""Submit"", ""Type"": ""Start"" },
    { ""Name"": ""ManagerApproval"", ""Type"": ""Approval"" },
    { ""Name"": ""DirectorApproval"", ""Type"": ""Approval"" },
    { ""Name"": ""Complete"", ""Type"": ""End"" }
  ]
}",
                IsActive = true,
                Version = 1,
                TenantId = DefaultTenantId
            },
            new WorkflowDefinition
            {
                Name = "Leave Request Approval",
                Description = "Employee leave request approval workflow",
                Category = "Approval",
                ConfigurationJson = @"{
  ""Steps"": [
    { ""Name"": ""Submit"", ""Type"": ""Start"" },
    { ""Name"": ""ManagerApproval"", ""Type"": ""Approval"" },
    { ""Name"": ""HRApproval"", ""Type"": ""Approval"" },
    { ""Name"": ""Complete"", ""Type"": ""End"" }
  ]
}",
                IsActive = true,
                Version = 1,
                TenantId = DefaultTenantId
            }
        };

        foreach (var definition in definitions)
        {
            definition.Id = Guid.NewGuid();
            definition.CreatedAt = DateTime.UtcNow;
            definition.CreatedBy = AppConstants.SystemUser;
        }

        context.WorkflowDefinitions.AddRange(definitions);
        await context.SaveChangesAsync();
    }
}
