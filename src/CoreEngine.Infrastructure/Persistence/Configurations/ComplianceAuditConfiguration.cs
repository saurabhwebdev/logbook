using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class ComplianceAuditConfiguration : IEntityTypeConfiguration<ComplianceAudit>
{
    public void Configure(EntityTypeBuilder<ComplianceAudit> builder)
    {
        builder.ToTable("ComplianceAudits");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.AuditNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.AuditorName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.AuditType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Findings)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(e => e.ComplianceStatus)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Compliant");

        builder.Property(e => e.CorrectiveActions)
            .HasMaxLength(4000);

        builder.Property(e => e.EvidenceReferences)
            .HasMaxLength(1000);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Open");

        builder.Property(e => e.Notes)
            .HasMaxLength(4000);

        builder.HasOne(e => e.ComplianceRequirement)
            .WithMany(r => r.Audits)
            .HasForeignKey(e => e.ComplianceRequirementId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.AuditNumber })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}
