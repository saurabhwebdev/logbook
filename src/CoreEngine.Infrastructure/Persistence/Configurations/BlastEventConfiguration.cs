using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class BlastEventConfiguration : IEntityTypeConfiguration<BlastEvent>
{
    public void Configure(EntityTypeBuilder<BlastEvent> builder)
    {
        builder.ToTable("BlastEvents");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.BlastNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(b => b.BlastType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.Location)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(b => b.DrillingPattern)
            .HasMaxLength(200);

        builder.Property(b => b.ExplosiveType)
            .HasMaxLength(200);

        builder.Property(b => b.DetonatorType)
            .HasMaxLength(200);

        builder.Property(b => b.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Planned");

        builder.Property(b => b.BlastDesignNotes)
            .HasMaxLength(4000);

        builder.Property(b => b.SupervisorName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.LicensedBlasterName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.PostBlastInspection)
            .HasMaxLength(2000);

        builder.Property(b => b.PostBlastNotes)
            .HasMaxLength(4000);

        builder.Property(b => b.FragmentationQuality)
            .HasMaxLength(50);

        builder.Property(b => b.TotalExplosivesKg)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(b => b.MineSite)
            .WithMany()
            .HasForeignKey(b => b.MineSiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.MineArea)
            .WithMany()
            .HasForeignKey(b => b.MineAreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => new { b.TenantId, b.BlastNumber })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}
