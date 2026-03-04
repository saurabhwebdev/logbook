using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class ProductionLogConfiguration : IEntityTypeConfiguration<ProductionLog>
{
    public void Configure(EntityTypeBuilder<ProductionLog> builder)
    {
        builder.ToTable("ProductionLogs");

        builder.Property(p => p.LogNumber).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Material).HasMaxLength(50).IsRequired();
        builder.Property(p => p.ShiftName).HasMaxLength(100);
        builder.Property(p => p.SourceLocation).HasMaxLength(300);
        builder.Property(p => p.DestinationLocation).HasMaxLength(300);
        builder.Property(p => p.QuantityTonnes).HasColumnType("decimal(18,2)");
        builder.Property(p => p.QuantityBCM).HasColumnType("decimal(18,2)");
        builder.Property(p => p.EquipmentUsed).HasMaxLength(500);
        builder.Property(p => p.OperatorName).HasMaxLength(200);
        builder.Property(p => p.Status).HasMaxLength(30).IsRequired().HasDefaultValue("Draft");
        builder.Property(p => p.Notes).HasMaxLength(2000);
        builder.Property(p => p.VerifiedBy).HasMaxLength(200);

        builder.HasOne(p => p.MineSite).WithMany().HasForeignKey(p => p.MineSiteId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.MineArea).WithMany().HasForeignKey(p => p.MineAreaId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.TenantId, p.LogNumber }).IsUnique().HasFilter("IsDeleted = 0");
    }
}
