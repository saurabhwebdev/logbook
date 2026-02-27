using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class ReportDefinitionConfiguration : IEntityTypeConfiguration<ReportDefinition>
{
    public void Configure(EntityTypeBuilder<ReportDefinition> builder)
    {
        builder.ToTable("ReportDefinitions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(256).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.EntityType).HasMaxLength(100).IsRequired();
        builder.Property(e => e.ColumnsJson).HasMaxLength(4000).IsRequired();
        builder.Property(e => e.FiltersJson).HasMaxLength(4000);
        builder.Property(e => e.ExportFormat).HasMaxLength(20).IsRequired();
        builder.HasIndex(e => new { e.TenantId, e.Name }).IsUnique();
    }
}
