using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class ShiftInstanceConfiguration : IEntityTypeConfiguration<ShiftInstance>
{
    public void Configure(EntityTypeBuilder<ShiftInstance> builder)
    {
        builder.ToTable("ShiftInstances");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Date)
            .IsRequired();

        builder.Property(s => s.SupervisorName)
            .HasMaxLength(200);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Scheduled");

        builder.Property(s => s.WeatherConditions)
            .HasMaxLength(500);

        builder.Property(s => s.Notes)
            .HasMaxLength(2000);

        // FK to ShiftDefinition
        builder.HasOne(s => s.ShiftDefinition)
            .WithMany(d => d.ShiftInstances)
            .HasForeignKey(s => s.ShiftDefinitionId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // FK to MineSite
        builder.HasOne(s => s.MineSite)
            .WithMany()
            .HasForeignKey(s => s.MineSiteId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // Index on (MineSiteId, Date)
        builder.HasIndex(s => new { s.MineSiteId, s.Date });
    }
}
