using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class StateDefinitionConfiguration : IEntityTypeConfiguration<StateDefinition>
{
    public void Configure(EntityTypeBuilder<StateDefinition> builder)
    {
        builder.ToTable("StateDefinitions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EntityType).HasMaxLength(256).IsRequired();
        builder.Property(e => e.StateName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Color).HasMaxLength(50);
        builder.HasIndex(e => new { e.TenantId, e.EntityType, e.StateName }).IsUnique();
    }
}
