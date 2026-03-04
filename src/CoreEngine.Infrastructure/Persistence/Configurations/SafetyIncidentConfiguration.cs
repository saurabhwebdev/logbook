using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class SafetyIncidentConfiguration : IEntityTypeConfiguration<SafetyIncident>
{
    public void Configure(EntityTypeBuilder<SafetyIncident> builder)
    {
        builder.ToTable("SafetyIncidents");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.IncidentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.IncidentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Severity)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Low");

        builder.Property(s => s.Location)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.Description)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(s => s.ImmediateActions)
            .HasMaxLength(2000);

        builder.Property(s => s.ReportedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.InjuredPersonName)
            .HasMaxLength(200);

        builder.Property(s => s.InjuredPersonRole)
            .HasMaxLength(200);

        builder.Property(s => s.InjuryType)
            .HasMaxLength(100);

        builder.Property(s => s.BodyPartAffected)
            .HasMaxLength(200);

        builder.Property(s => s.RegulatoryReference)
            .HasMaxLength(200);

        builder.Property(s => s.WitnessNames)
            .HasMaxLength(1000);

        builder.Property(s => s.RootCause)
            .HasMaxLength(2000);

        builder.Property(s => s.ContributingFactors)
            .HasMaxLength(2000);

        builder.Property(s => s.CorrectiveActions)
            .HasMaxLength(2000);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Open");

        builder.HasOne(s => s.MineSite)
            .WithMany()
            .HasForeignKey(s => s.MineSiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.MineArea)
            .WithMany()
            .HasForeignKey(s => s.MineAreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => new { s.TenantId, s.IncidentNumber })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}
