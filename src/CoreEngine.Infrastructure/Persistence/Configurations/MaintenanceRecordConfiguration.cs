using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class MaintenanceRecordConfiguration : IEntityTypeConfiguration<MaintenanceRecord>
{
    public void Configure(EntityTypeBuilder<MaintenanceRecord> builder)
    {
        builder.ToTable("MaintenanceRecords");

        builder.Property(e => e.WorkOrderNumber).HasMaxLength(50).IsRequired();
        builder.Property(e => e.MaintenanceType).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Priority).HasMaxLength(20).IsRequired();
        builder.Property(e => e.Title).HasMaxLength(300).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(4000);
        builder.Property(e => e.PerformedBy).HasMaxLength(200);
        builder.Property(e => e.Status).HasMaxLength(30).IsRequired();
        builder.Property(e => e.PartsUsed).HasMaxLength(2000);
        builder.Property(e => e.Findings).HasMaxLength(4000);
        builder.Property(e => e.ActionsTaken).HasMaxLength(4000);
        builder.Property(e => e.Notes).HasMaxLength(2000);
        builder.Property(e => e.LaborCost).HasColumnType("decimal(18,2)");
        builder.Property(e => e.PartsCost).HasColumnType("decimal(18,2)");

        builder.HasOne(e => e.Equipment).WithMany(eq => eq.MaintenanceRecords).HasForeignKey(e => e.EquipmentId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.WorkOrderNumber }).IsUnique().HasFilter("IsDeleted = 0");
    }
}
