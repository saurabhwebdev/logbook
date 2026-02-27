using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class StateTransitionDefinitionConfiguration : IEntityTypeConfiguration<StateTransitionDefinition>
{
    public void Configure(EntityTypeBuilder<StateTransitionDefinition> builder)
    {
        builder.ToTable("StateTransitionDefinitions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EntityType).HasMaxLength(256).IsRequired();
        builder.Property(e => e.FromState).HasMaxLength(100).IsRequired();
        builder.Property(e => e.ToState).HasMaxLength(100).IsRequired();
        builder.Property(e => e.TriggerName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.RequiredPermission).HasMaxLength(256);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.HasIndex(e => new { e.TenantId, e.EntityType, e.FromState, e.TriggerName }).IsUnique();
    }
}
