using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class HelpArticleConfiguration : IEntityTypeConfiguration<HelpArticle>
{
    public void Configure(EntityTypeBuilder<HelpArticle> builder)
    {
        builder.ToTable("HelpArticles");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Slug).HasMaxLength(200).IsRequired();
        builder.Property(e => e.ModuleKey).HasMaxLength(100);
        builder.Property(e => e.Content).IsRequired();
        builder.Property(e => e.Category).HasMaxLength(100);
        builder.Property(e => e.SortOrder).HasDefaultValue(0);
        builder.Property(e => e.IsPublished).HasDefaultValue(true);
        builder.Property(e => e.Tags).HasMaxLength(500);
        builder.HasIndex(e => new { e.TenantId, e.Slug }).IsUnique();
    }
}
