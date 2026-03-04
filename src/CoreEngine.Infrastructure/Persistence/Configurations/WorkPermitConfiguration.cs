using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class WorkPermitConfiguration : IEntityTypeConfiguration<WorkPermit>
{
    public void Configure(EntityTypeBuilder<WorkPermit> builder)
    {
        builder.ToTable("WorkPermits");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.PermitNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(w => w.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(w => w.PermitType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(w => w.RequestedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Location)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(w => w.WorkDescription)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(w => w.HazardsIdentified)
            .HasMaxLength(4000);

        builder.Property(w => w.ControlMeasures)
            .HasMaxLength(4000);

        builder.Property(w => w.PPERequired)
            .HasMaxLength(2000);

        builder.Property(w => w.EmergencyProcedures)
            .HasMaxLength(4000);

        builder.Property(w => w.GasTestResults)
            .HasMaxLength(2000);

        builder.Property(w => w.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Draft");

        builder.Property(w => w.ApprovedBy)
            .HasMaxLength(200);

        builder.Property(w => w.ClosedBy)
            .HasMaxLength(200);

        builder.Property(w => w.RejectionReason)
            .HasMaxLength(2000);

        builder.Property(w => w.Notes)
            .HasMaxLength(4000);

        builder.HasOne(w => w.MineSite)
            .WithMany()
            .HasForeignKey(w => w.MineSiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.MineArea)
            .WithMany()
            .HasForeignKey(w => w.MineAreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(w => new { w.TenantId, w.PermitNumber })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}
