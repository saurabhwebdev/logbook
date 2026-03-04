using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class ShiftHandoverConfiguration : IEntityTypeConfiguration<ShiftHandover>
{
    public void Configure(EntityTypeBuilder<ShiftHandover> builder)
    {
        builder.ToTable("ShiftHandovers");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.HandoverDateTime)
            .IsRequired();

        builder.Property(h => h.SafetyIssues)
            .HasMaxLength(2000);

        builder.Property(h => h.OngoingOperations)
            .HasMaxLength(2000);

        builder.Property(h => h.PendingTasks)
            .HasMaxLength(2000);

        builder.Property(h => h.EquipmentStatus)
            .HasMaxLength(2000);

        builder.Property(h => h.EnvironmentalConditions)
            .HasMaxLength(1000);

        builder.Property(h => h.GeneralRemarks)
            .HasMaxLength(2000);

        builder.Property(h => h.HandedOverBy)
            .HasMaxLength(200);

        builder.Property(h => h.ReceivedBy)
            .HasMaxLength(200);

        builder.Property(h => h.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Draft");

        // FK to OutgoingShiftInstance
        builder.HasOne(h => h.OutgoingShiftInstance)
            .WithMany(s => s.Handovers)
            .HasForeignKey(h => h.OutgoingShiftInstanceId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // FK to IncomingShiftInstance (nullable)
        builder.HasOne(h => h.IncomingShiftInstance)
            .WithMany()
            .HasForeignKey(h => h.IncomingShiftInstanceId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // FK to MineSite
        builder.HasOne(h => h.MineSite)
            .WithMany()
            .HasForeignKey(h => h.MineSiteId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
