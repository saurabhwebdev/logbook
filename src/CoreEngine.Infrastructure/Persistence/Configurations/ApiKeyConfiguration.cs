using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.ToTable("ApiKeys");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(256).IsRequired();
        builder.Property(e => e.KeyHash).HasMaxLength(500).IsRequired();
        builder.Property(e => e.KeyPrefix).HasMaxLength(20).IsRequired();
        builder.Property(e => e.Scopes).HasMaxLength(2000);
        builder.HasIndex(e => new { e.TenantId, e.Name }).IsUnique();
    }
}
