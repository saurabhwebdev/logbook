using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class ExplosiveUsageConfiguration : IEntityTypeConfiguration<ExplosiveUsage>
{
    public void Configure(EntityTypeBuilder<ExplosiveUsage> builder)
    {
        builder.ToTable("ExplosiveUsages");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ExplosiveName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.BatchNumber)
            .HasMaxLength(100);

        builder.Property(e => e.Unit)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("kg");

        builder.Property(e => e.MagazineSource)
            .HasMaxLength(200);

        builder.Property(e => e.IssuedBy)
            .HasMaxLength(200);

        builder.Property(e => e.ReceivedBy)
            .HasMaxLength(200);

        builder.Property(e => e.Notes)
            .HasMaxLength(2000);

        builder.Property(e => e.QuantityIssued)
            .HasColumnType("decimal(18,3)");

        builder.Property(e => e.QuantityUsed)
            .HasColumnType("decimal(18,3)");

        builder.Property(e => e.QuantityReturned)
            .HasColumnType("decimal(18,3)");

        builder.HasOne(e => e.BlastEvent)
            .WithMany(b => b.ExplosiveUsages)
            .HasForeignKey(e => e.BlastEventId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
