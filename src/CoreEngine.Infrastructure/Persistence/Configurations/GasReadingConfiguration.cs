using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class GasReadingConfiguration : IEntityTypeConfiguration<GasReading>
{
    public void Configure(EntityTypeBuilder<GasReading> builder)
    {
        builder.ToTable("GasReadings");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ReadingNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.GasType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Concentration)
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.Unit)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.ThresholdTWA)
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.ThresholdSTEL)
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.ThresholdCeiling)
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.LocationDescription)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.RecordedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.InstrumentId)
            .HasMaxLength(200);

        builder.Property(e => e.ActionTaken)
            .HasMaxLength(4000);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Normal");

        builder.Property(e => e.Notes)
            .HasMaxLength(4000);

        builder.HasOne(e => e.MineSite)
            .WithMany()
            .HasForeignKey(e => e.MineSiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.MineArea)
            .WithMany()
            .HasForeignKey(e => e.MineAreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.ReadingNumber })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}
