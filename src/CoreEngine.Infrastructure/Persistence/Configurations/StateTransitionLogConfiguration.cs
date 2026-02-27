using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class StateTransitionLogConfiguration : IEntityTypeConfiguration<StateTransitionLog>
{
    public void Configure(EntityTypeBuilder<StateTransitionLog> builder)
    {
        builder.ToTable("StateTransitionLogs");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EntityType).HasMaxLength(256).IsRequired();
        builder.Property(e => e.EntityId).HasMaxLength(256).IsRequired();
        builder.Property(e => e.FromState).HasMaxLength(100).IsRequired();
        builder.Property(e => e.ToState).HasMaxLength(100).IsRequired();
        builder.Property(e => e.TriggerName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.PerformedBy).HasMaxLength(256);
        builder.Property(e => e.Comments).HasMaxLength(1000);
        builder.HasIndex(e => new { e.EntityType, e.EntityId });
    }
}
