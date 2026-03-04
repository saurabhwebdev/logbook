using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class InspectionFindingConfiguration : IEntityTypeConfiguration<InspectionFinding>
{
    public void Configure(EntityTypeBuilder<InspectionFinding> builder)
    {
        builder.ToTable("InspectionFindings");

        builder.Property(e => e.FindingNumber).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Category).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Severity).HasMaxLength(20).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(4000).IsRequired();
        builder.Property(e => e.Location).HasMaxLength(300);
        builder.Property(e => e.RecommendedAction).HasMaxLength(2000);
        builder.Property(e => e.AssignedTo).HasMaxLength(200);
        builder.Property(e => e.Status).HasMaxLength(30).IsRequired();
        builder.Property(e => e.ClosureNotes).HasMaxLength(2000);

        builder.HasOne(e => e.Inspection)
            .WithMany(i => i.Findings)
            .HasForeignKey(e => e.InspectionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
