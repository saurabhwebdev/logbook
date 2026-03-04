using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class GeotechnicalAssessmentConfiguration : IEntityTypeConfiguration<GeotechnicalAssessment>
{
    public void Configure(EntityTypeBuilder<GeotechnicalAssessment> builder)
    {
        builder.ToTable("GeotechnicalAssessments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.AssessmentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.AssessmentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.AssessorName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Location)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.RockMassRating)
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.SlopeAngle)
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.WaterTableDepth)
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.GroundCondition)
            .HasMaxLength(50);

        builder.Property(e => e.StabilityStatus)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Stable");

        builder.Property(e => e.RecommendedActions)
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

        builder.HasIndex(e => new { e.TenantId, e.AssessmentNumber })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}
