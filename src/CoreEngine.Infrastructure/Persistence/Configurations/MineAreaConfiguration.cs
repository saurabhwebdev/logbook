using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class MineAreaConfiguration : IEntityTypeConfiguration<MineArea>
{
    public void Configure(EntityTypeBuilder<MineArea> builder)
    {
        builder.ToTable("MineAreas");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Code)
            .HasMaxLength(50);

        builder.Property(a => a.AreaType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Description)
            .HasMaxLength(500);

        builder.Property(a => a.IsActive)
            .HasDefaultValue(true);

        builder.Property(a => a.SortOrder)
            .HasDefaultValue(0);

        // FK to MineSite
        builder.HasOne(a => a.MineSite)
            .WithMany(m => m.MineAreas)
            .HasForeignKey(a => a.MineSiteId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // Self-referencing FK for hierarchy
        builder.HasOne(a => a.ParentArea)
            .WithMany(a => a.ChildAreas)
            .HasForeignKey(a => a.ParentAreaId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Unique name per mine site
        builder.HasIndex(a => new { a.MineSiteId, a.Name })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}
