using System.Text.Json;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Common;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace CoreEngine.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ITenantContext _tenantContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeService _dateTimeService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantContext tenantContext,
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService)
        : base(options)
    {
        _tenantContext = tenantContext;
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // Phase 2
    public DbSet<SystemConfiguration> SystemConfigurations => Set<SystemConfiguration>();
    public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<StateDefinition> StateDefinitions => Set<StateDefinition>();
    public DbSet<StateTransitionDefinition> StateTransitionDefinitions => Set<StateTransitionDefinition>();
    public DbSet<StateTransitionLog> StateTransitionLogs => Set<StateTransitionLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        ApplyGlobalQueryFilters(modelBuilder);
    }

    private void ApplyGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            var isTenantScoped = typeof(TenantScopedEntity).IsAssignableFrom(clrType);
            var isSoftDeletable = typeof(ISoftDeletable).IsAssignableFrom(clrType);

            if (!isTenantScoped && !isSoftDeletable)
                continue;

            var parameter = Expression.Parameter(clrType, "e");
            Expression? filterBody = null;

            if (isSoftDeletable)
            {
                var isDeletedProp = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var softDeleteFilter = Expression.Equal(isDeletedProp, Expression.Constant(false));
                filterBody = softDeleteFilter;
            }

            if (isTenantScoped)
            {
                var tenantIdProp = Expression.Property(parameter, nameof(TenantScopedEntity.TenantId));

                var contextField = Expression.Field(
                    Expression.Constant(this),
                    typeof(ApplicationDbContext).GetField(
                        nameof(_tenantContext),
                        BindingFlags.NonPublic | BindingFlags.Instance)!);
                var contextTenantId = Expression.Property(contextField, nameof(ITenantContext.TenantId));

                var tenantMatch = Expression.Equal(tenantIdProp, contextTenantId);
                var superAdminBypass = Expression.Equal(contextTenantId, Expression.Constant(Guid.Empty));
                var tenantFilter = Expression.OrElse(tenantMatch, superAdminBypass);

                filterBody = filterBody is not null
                    ? Expression.AndAlso(filterBody, tenantFilter)
                    : tenantFilter;
            }

            if (filterBody is not null)
            {
                var lambda = Expression.Lambda(filterBody, parameter);
                entityType.SetQueryFilter(lambda);
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = _dateTimeService.UtcNow;
        var userId = _currentUserService.UserId;

        // 1. Collect audit entries BEFORE modifying tracked entities
        var auditEntries = CollectAuditEntries();

        // 2. Process entity changes (soft delete, audit fields, tenant stamping)
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Deleted && entry.Entity is ISoftDeletable softDeletable)
            {
                entry.State = EntityState.Modified;
                softDeletable.IsDeleted = true;

                if (entry.Entity is BaseEntity deletedBaseEntity)
                {
                    deletedBaseEntity.ModifiedAt = utcNow;
                    deletedBaseEntity.ModifiedBy = userId;
                }
            }

            if (entry.State == EntityState.Added)
            {
                if (entry.Entity is BaseEntity addedEntity)
                {
                    if (addedEntity.Id == Guid.Empty)
                    {
                        addedEntity.Id = Guid.NewGuid();
                    }

                    addedEntity.CreatedAt = utcNow;
                    addedEntity.CreatedBy = userId;
                }

                if (entry.Entity is TenantScopedEntity tenantEntity
                    && tenantEntity.TenantId == Guid.Empty
                    && _tenantContext.TenantId != Guid.Empty)
                {
                    tenantEntity.TenantId = _tenantContext.TenantId;
                }
            }

            if (entry.State == EntityState.Modified && entry.Entity is BaseEntity modifiedEntity)
            {
                modifiedEntity.ModifiedAt = utcNow;
                modifiedEntity.ModifiedBy = userId;

                entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
            }
        }

        // 3. Add audit log entries to the context (they'll be saved in the same transaction)
        foreach (var auditEntry in auditEntries)
        {
            AuditLogs.Add(auditEntry);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private List<AuditLog> CollectAuditEntries()
    {
        var auditLogs = new List<AuditLog>();

        foreach (var entry in ChangeTracker.Entries())
        {
            // Skip entities that shouldn't be audited
            if (entry.Entity is AuditLog || entry.Entity is RefreshToken)
                continue;

            if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
                continue;

            var auditLog = new AuditLog
            {
                EntityName = entry.Entity.GetType().Name,
                Action = entry.State switch
                {
                    EntityState.Added => "Create",
                    EntityState.Modified => "Update",
                    EntityState.Deleted => "Delete",
                    _ => "Unknown"
                },
                TenantId = _tenantContext.TenantId == Guid.Empty ? null : _tenantContext.TenantId,
                UserId = _currentUserService.UserId,
                IpAddress = _currentUserService.IpAddress,
                Timestamp = _dateTimeService.UtcNow
            };

            var idProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "Id");
            auditLog.EntityId = idProperty?.CurrentValue?.ToString() ?? "Unknown";

            switch (entry.State)
            {
                case EntityState.Added:
                {
                    var newValues = new Dictionary<string, object?>();
                    foreach (var property in entry.Properties)
                    {
                        newValues[property.Metadata.Name] = property.CurrentValue;
                    }
                    auditLog.NewValues = JsonSerializer.Serialize(newValues);
                    break;
                }
                case EntityState.Modified:
                {
                    var oldValues = new Dictionary<string, object?>();
                    var newValues = new Dictionary<string, object?>();
                    foreach (var property in entry.Properties)
                    {
                        if (property.IsModified)
                        {
                            oldValues[property.Metadata.Name] = property.OriginalValue;
                            newValues[property.Metadata.Name] = property.CurrentValue;
                        }
                    }
                    auditLog.OldValues = JsonSerializer.Serialize(oldValues);
                    auditLog.NewValues = JsonSerializer.Serialize(newValues);
                    break;
                }
                case EntityState.Deleted:
                {
                    var oldValues = new Dictionary<string, object?>();
                    foreach (var property in entry.Properties)
                    {
                        oldValues[property.Metadata.Name] = property.OriginalValue;
                    }
                    auditLog.OldValues = JsonSerializer.Serialize(oldValues);
                    break;
                }
            }

            auditLogs.Add(auditLog);
        }

        return auditLogs;
    }
}
