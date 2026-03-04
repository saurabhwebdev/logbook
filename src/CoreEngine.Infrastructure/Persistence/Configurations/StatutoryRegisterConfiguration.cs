using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class StatutoryRegisterConfiguration : IEntityTypeConfiguration<StatutoryRegister>
{
    public void Configure(EntityTypeBuilder<StatutoryRegister> builder)
    {
        builder.ToTable("StatutoryRegisters");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Code)
            .HasMaxLength(50);

        builder.Property(s => s.RegisterType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Description)
            .HasMaxLength(1000);

        builder.Property(s => s.Jurisdiction)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.IsRequired)
            .HasDefaultValue(true);

        builder.Property(s => s.RetentionYears)
            .HasDefaultValue(5);

        builder.Property(s => s.IsActive)
            .HasDefaultValue(true);

        builder.Property(s => s.SortOrder)
            .HasDefaultValue(0);

        // FK to MineSite
        builder.HasOne(s => s.MineSite)
            .WithMany()
            .HasForeignKey(s => s.MineSiteId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // Navigation to Entries
        builder.HasMany(s => s.Entries)
            .WithOne(e => e.StatutoryRegister)
            .HasForeignKey(e => e.StatutoryRegisterId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique name per mine site
        builder.HasIndex(s => new { s.MineSiteId, s.Name })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}
