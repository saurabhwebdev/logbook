using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class DispatchRecordConfiguration : IEntityTypeConfiguration<DispatchRecord>
{
    public void Configure(EntityTypeBuilder<DispatchRecord> builder)
    {
        builder.ToTable("DispatchRecords");

        builder.Property(d => d.DispatchNumber).HasMaxLength(50).IsRequired();
        builder.Property(d => d.VehicleNumber).HasMaxLength(100).IsRequired();
        builder.Property(d => d.DriverName).HasMaxLength(200);
        builder.Property(d => d.Material).HasMaxLength(100).IsRequired();
        builder.Property(d => d.SourceLocation).HasMaxLength(300).IsRequired();
        builder.Property(d => d.DestinationLocation).HasMaxLength(300).IsRequired();
        builder.Property(d => d.WeighbridgeTicketNumber).HasMaxLength(100);
        builder.Property(d => d.GrossWeight).HasColumnType("decimal(18,2)");
        builder.Property(d => d.TareWeight).HasColumnType("decimal(18,2)");
        builder.Property(d => d.NetWeight).HasColumnType("decimal(18,2)");
        builder.Property(d => d.Unit).HasMaxLength(30).IsRequired().HasDefaultValue("Tonnes");
        builder.Property(d => d.Status).HasMaxLength(30).IsRequired().HasDefaultValue("Loading");
        builder.Property(d => d.Notes).HasMaxLength(2000);

        builder.HasOne(d => d.MineSite).WithMany().HasForeignKey(d => d.MineSiteId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => new { d.TenantId, d.DispatchNumber }).IsUnique().HasFilter("IsDeleted = 0");
    }
}
