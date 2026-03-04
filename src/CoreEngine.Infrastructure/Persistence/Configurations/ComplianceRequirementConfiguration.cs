using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class ComplianceRequirementConfiguration : IEntityTypeConfiguration<ComplianceRequirement>
{
    public void Configure(EntityTypeBuilder<ComplianceRequirement> builder)
    {
        builder.ToTable("ComplianceRequirements");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Jurisdiction)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(e => e.RegulatoryBody)
            .HasMaxLength(200);

        builder.Property(e => e.ReferenceDocument)
            .HasMaxLength(500);

        builder.Property(e => e.Frequency)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("AsRequired");

        builder.Property(e => e.ResponsibleRole)
            .HasMaxLength(200);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Pending");

        builder.Property(e => e.Priority)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Medium");

        builder.Property(e => e.PenaltyForNonCompliance)
            .HasMaxLength(2000);

        builder.Property(e => e.Notes)
            .HasMaxLength(4000);

        builder.HasOne(e => e.MineSite)
            .WithMany()
            .HasForeignKey(e => e.MineSiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.Code })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}
