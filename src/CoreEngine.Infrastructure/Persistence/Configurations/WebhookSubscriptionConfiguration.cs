using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class WebhookSubscriptionConfiguration : IEntityTypeConfiguration<WebhookSubscription>
{
    public void Configure(EntityTypeBuilder<WebhookSubscription> builder)
    {
        builder.ToTable("WebhookSubscriptions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(256).IsRequired();
        builder.Property(e => e.EndpointUrl).HasMaxLength(1000).IsRequired();
        builder.Property(e => e.Secret).HasMaxLength(500).IsRequired();
        builder.Property(e => e.EventTypes).HasMaxLength(2000).IsRequired();
        builder.HasIndex(e => new { e.TenantId, e.Name }).IsUnique();
    }
}
