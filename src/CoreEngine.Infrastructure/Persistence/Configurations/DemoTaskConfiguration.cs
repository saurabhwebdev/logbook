using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class DemoTaskConfiguration : IEntityTypeConfiguration<DemoTask>
{
    public void Configure(EntityTypeBuilder<DemoTask> builder)
    {
        builder.ToTable("DemoTasks");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).HasMaxLength(256).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(2000);
        builder.Property(e => e.CurrentState).HasMaxLength(100).IsRequired();
        builder.Property(e => e.AssignedTo).HasMaxLength(256);
        builder.Property(e => e.Priority).HasMaxLength(50).IsRequired();
        builder.HasIndex(e => new { e.TenantId, e.CurrentState });
    }
}
