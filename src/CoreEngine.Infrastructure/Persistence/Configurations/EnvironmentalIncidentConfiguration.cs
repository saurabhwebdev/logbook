using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class EnvironmentalIncidentConfiguration : IEntityTypeConfiguration<EnvironmentalIncident>
{
    public void Configure(EntityTypeBuilder<EnvironmentalIncident> builder)
    {
        builder.ToTable("EnvironmentalIncidents");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.IncidentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.IncidentType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Severity)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Location)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(e => e.ImpactAssessment)
            .HasMaxLength(4000);

        builder.Property(e => e.ContainmentActions)
            .HasMaxLength(4000);

        builder.Property(e => e.RemediationPlan)
            .HasMaxLength(4000);

        builder.Property(e => e.ReportedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.AuthorityReference)
            .HasMaxLength(200);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Open");

        builder.Property(e => e.ClosureNotes)
            .HasMaxLength(4000);

        builder.HasOne(e => e.MineSite)
            .WithMany()
            .HasForeignKey(e => e.MineSiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.IncidentNumber })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}
