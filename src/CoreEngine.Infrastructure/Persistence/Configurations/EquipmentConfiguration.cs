using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("Equipment");

        builder.Property(e => e.AssetNumber).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Category).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Make).HasMaxLength(100);
        builder.Property(e => e.Model).HasMaxLength(100);
        builder.Property(e => e.SerialNumber).HasMaxLength(100);
        builder.Property(e => e.Status).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Location).HasMaxLength(300);
        builder.Property(e => e.OperatorName).HasMaxLength(200);
        builder.Property(e => e.WarrantyInfo).HasMaxLength(500);
        builder.Property(e => e.Notes).HasMaxLength(2000);
        builder.Property(e => e.PurchaseCost).HasColumnType("decimal(18,2)");

        builder.HasOne(e => e.MineSite).WithMany().HasForeignKey(e => e.MineSiteId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.MineArea).WithMany().HasForeignKey(e => e.MineAreaId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.AssetNumber }).IsUnique().HasFilter("IsDeleted = 0");
    }
}
