using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class InspectionConfiguration : IEntityTypeConfiguration<Inspection>
{
    public void Configure(EntityTypeBuilder<Inspection> builder)
    {
        builder.ToTable("Inspections");

        builder.Property(e => e.InspectionNumber).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Title).HasMaxLength(300).IsRequired();
        builder.Property(e => e.InspectorName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.InspectorRole).HasMaxLength(100);
        builder.Property(e => e.Status).HasMaxLength(30).IsRequired();
        builder.Property(e => e.OverallRating).HasMaxLength(30);
        builder.Property(e => e.Summary).HasMaxLength(4000);
        builder.Property(e => e.WeatherConditions).HasMaxLength(200);
        builder.Property(e => e.SignedOffBy).HasMaxLength(200);

        builder.HasOne(e => e.InspectionTemplate)
            .WithMany(t => t.Inspections)
            .HasForeignKey(e => e.InspectionTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.MineSite)
            .WithMany()
            .HasForeignKey(e => e.MineSiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.MineArea)
            .WithMany()
            .HasForeignKey(e => e.MineAreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.InspectionNumber })
            .IsUnique()
            .HasFilter("IsDeleted = 0");
    }
}
