using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class RegisterEntryConfiguration : IEntityTypeConfiguration<RegisterEntry>
{
    public void Configure(EntityTypeBuilder<RegisterEntry> builder)
    {
        builder.ToTable("RegisterEntries");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EntryNumber)
            .IsRequired();

        builder.Property(e => e.EntryDate)
            .IsRequired();

        builder.Property(e => e.Subject)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Details)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(e => e.ReportedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.WitnessName)
            .HasMaxLength(200);

        builder.Property(e => e.ActionTaken)
            .HasMaxLength(2000);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Open");

        builder.Property(e => e.AmendmentReason)
            .HasMaxLength(1000);

        // FK to StatutoryRegister
        builder.HasOne(e => e.StatutoryRegister)
            .WithMany(s => s.Entries)
            .HasForeignKey(e => e.StatutoryRegisterId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // FK to MineSite
        builder.HasOne(e => e.MineSite)
            .WithMany()
            .HasForeignKey(e => e.MineSiteId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // FK to ShiftInstance (optional)
        builder.HasOne(e => e.ShiftInstance)
            .WithMany()
            .HasForeignKey(e => e.ShiftInstanceId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // FK to MineArea (optional)
        builder.HasOne(e => e.MineArea)
            .WithMany()
            .HasForeignKey(e => e.MineAreaId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Self-referencing FK to AmendmentOfEntry (optional)
        builder.HasOne(e => e.AmendmentOfEntry)
            .WithMany(e => e.Amendments)
            .HasForeignKey(e => e.AmendmentOfEntryId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Index on (StatutoryRegisterId, EntryNumber)
        builder.HasIndex(e => new { e.StatutoryRegisterId, e.EntryNumber })
            .HasFilter("[IsDeleted] = 0");
    }
}
