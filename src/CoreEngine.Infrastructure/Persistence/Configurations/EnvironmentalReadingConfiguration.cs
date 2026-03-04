using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class EnvironmentalReadingConfiguration : IEntityTypeConfiguration<EnvironmentalReading>
{
    public void Configure(EntityTypeBuilder<EnvironmentalReading> builder)
    {
        builder.ToTable("EnvironmentalReadings");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ReadingNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.ReadingType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Parameter)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Value)
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.Unit)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.ThresholdMin)
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.ThresholdMax)
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.MonitoringStation)
            .HasMaxLength(200);

        builder.Property(e => e.InstrumentUsed)
            .HasMaxLength(200);

        builder.Property(e => e.RecordedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.WeatherConditions)
            .HasMaxLength(500);

        builder.Property(e => e.Notes)
            .HasMaxLength(4000);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Normal");

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
