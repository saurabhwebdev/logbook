using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class SystemConfigurationConfiguration : IEntityTypeConfiguration<SystemConfiguration>
{
    public void Configure(EntityTypeBuilder<SystemConfiguration> builder)
    {
        builder.ToTable("SystemConfigurations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Key).HasMaxLength(256).IsRequired();
        builder.Property(e => e.Value).HasMaxLength(4000).IsRequired();
        builder.Property(e => e.Category).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.DataType).HasMaxLength(50).IsRequired();
        builder.HasIndex(e => new { e.TenantId, e.Key }).IsUnique();
    }
}
