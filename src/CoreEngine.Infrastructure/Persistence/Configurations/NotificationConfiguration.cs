using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).HasMaxLength(256).IsRequired();
        builder.Property(e => e.Message).HasMaxLength(2000).IsRequired();
        builder.Property(e => e.Type).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Link).HasMaxLength(500);
        builder.HasIndex(e => new { e.RecipientUserId, e.IsRead });
        builder.HasIndex(e => e.CreatedAt);
    }
}
