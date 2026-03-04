using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class ShiftDefinitionConfiguration : IEntityTypeConfiguration<ShiftDefinition>
{
    public void Configure(EntityTypeBuilder<ShiftDefinition> builder)
    {
        builder.ToTable("ShiftDefinitions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Code)
            .HasMaxLength(20);

        builder.Property(s => s.StartTime)
            .IsRequired();

        builder.Property(s => s.EndTime)
            .IsRequired();

        builder.Property(s => s.ShiftOrder)
            .HasDefaultValue(0);

        builder.Property(s => s.Color)
            .HasMaxLength(20);

        builder.Property(s => s.IsActive)
            .HasDefaultValue(true);

        // FK to MineSite
        builder.HasOne(s => s.MineSite)
            .WithMany()
            .HasForeignKey(s => s.MineSiteId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // Navigation to ShiftInstances
        builder.HasMany(s => s.ShiftInstances)
            .WithOne(i => i.ShiftDefinition)
            .HasForeignKey(i => i.ShiftDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique name per mine site
        builder.HasIndex(s => new { s.MineSiteId, s.Name })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}
