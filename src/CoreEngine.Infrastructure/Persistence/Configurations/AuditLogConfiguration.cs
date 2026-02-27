using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        // bigint IDENTITY primary key
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedOnAdd();

        builder.Property(a => a.UserId)
            .HasMaxLength(100);

        builder.Property(a => a.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.EntityName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.EntityId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.OldValues)
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.NewValues)
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.IpAddress)
            .HasMaxLength(50);

        builder.Property(a => a.UserAgent)
            .HasMaxLength(500);

        builder.Property(a => a.Timestamp)
            .IsRequired();

        // Index on (TenantId, Timestamp) descending for efficient tenant-scoped queries
        builder.HasIndex(a => new { a.TenantId, a.Timestamp })
            .IsDescending(false, true);

        // Index on (EntityName, EntityId) for entity-specific audit trail lookups
        builder.HasIndex(a => new { a.EntityName, a.EntityId });

        // NO query filters -- audit logs must always be visible regardless of
        // tenant context or soft-delete state. AuditLog does not inherit BaseEntity
        // and does not implement ISoftDeletable or TenantScopedEntity, so the
        // global filter loop in ApplicationDbContext naturally skips it.
    }
}
