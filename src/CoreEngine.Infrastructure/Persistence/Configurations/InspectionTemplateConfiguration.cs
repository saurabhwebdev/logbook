using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class InspectionTemplateConfiguration : IEntityTypeConfiguration<InspectionTemplate>
{
    public void Configure(EntityTypeBuilder<InspectionTemplate> builder)
    {
        builder.ToTable("InspectionTemplates");

        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Code).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Category).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.Frequency).HasMaxLength(30).IsRequired();

        builder.HasIndex(e => new { e.TenantId, e.Code })
            .IsUnique()
            .HasFilter("IsDeleted = 0");
    }
}
