using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Module)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(300);

        // Unique index on (Module, Action)
        builder.HasIndex(p => new { p.Module, p.Action })
            .IsUnique();

        // Ignore the computed FullPermission property
        builder.Ignore(p => p.FullPermission);

        // Permission extends BaseEntity (ISoftDeletable) but is NOT TenantScopedEntity.
        // Explicit soft-delete query filter for clarity.
        builder.HasQueryFilter(p => !p.IsDeleted);

        // Navigation collection
        builder.HasMany(p => p.RolePermissions)
            .WithOne(rp => rp.Permission)
            .HasForeignKey(rp => rp.PermissionId);
    }
}
