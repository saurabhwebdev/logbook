using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class SurveyRecordConfiguration : IEntityTypeConfiguration<SurveyRecord>
{
    public void Configure(EntityTypeBuilder<SurveyRecord> builder)
    {
        builder.ToTable("SurveyRecords");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.SurveyNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.SurveyType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.SurveyorName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.SurveyorLicense)
            .HasMaxLength(200);

        builder.Property(e => e.Location)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Easting)
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.Northing)
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.Elevation)
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.Datum)
            .HasMaxLength(100);

        builder.Property(e => e.CoordinateSystem)
            .HasMaxLength(200);

        builder.Property(e => e.EquipmentUsed)
            .HasMaxLength(500);

        builder.Property(e => e.Accuracy)
            .HasMaxLength(100);

        builder.Property(e => e.VolumeCalculated)
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.AreaCalculated)
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.Findings)
            .HasMaxLength(4000);

        builder.Property(e => e.Notes)
            .HasMaxLength(4000);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Draft");

        builder.HasOne(e => e.MineSite)
            .WithMany()
            .HasForeignKey(e => e.MineSiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.MineArea)
            .WithMany()
            .HasForeignKey(e => e.MineAreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.SurveyNumber })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}
