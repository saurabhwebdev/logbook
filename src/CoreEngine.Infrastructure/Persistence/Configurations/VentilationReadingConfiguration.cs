using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class VentilationReadingConfiguration : IEntityTypeConfiguration<VentilationReading>
{
    public void Configure(EntityTypeBuilder<VentilationReading> builder)
    {
        builder.ToTable("VentilationReadings");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ReadingNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.LocationDescription)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.AirflowVelocity)
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.AirflowVolume)
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.Temperature)
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.Humidity)
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.BarometricPressure)
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.RecordedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.InstrumentUsed)
            .HasMaxLength(200);

        builder.Property(e => e.DoorStatus)
            .HasMaxLength(50);

        builder.Property(e => e.FanStatus)
            .HasMaxLength(50);

        builder.Property(e => e.VentilationStatus)
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
