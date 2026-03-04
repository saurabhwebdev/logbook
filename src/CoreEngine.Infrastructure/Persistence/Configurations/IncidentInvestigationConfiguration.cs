using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class IncidentInvestigationConfiguration : IEntityTypeConfiguration<IncidentInvestigation>
{
    public void Configure(EntityTypeBuilder<IncidentInvestigation> builder)
    {
        builder.ToTable("IncidentInvestigations");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvestigatorName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Methodology)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.Findings)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(i => i.RootCauseAnalysis)
            .HasMaxLength(4000);

        builder.Property(i => i.Recommendations)
            .HasMaxLength(2000);

        builder.Property(i => i.PreventiveMeasures)
            .HasMaxLength(2000);

        builder.Property(i => i.EvidenceReferences)
            .HasMaxLength(1000);

        builder.Property(i => i.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("InProgress");

        builder.HasOne(i => i.SafetyIncident)
            .WithMany(s => s.Investigations)
            .HasForeignKey(i => i.SafetyIncidentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
