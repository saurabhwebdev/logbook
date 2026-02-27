using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Subdomain)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(t => t.Subdomain)
            .IsUnique();

        builder.Property(t => t.ConnectionString)
            .HasMaxLength(500);

        builder.Property(t => t.Settings)
            .HasColumnType("nvarchar(max)");

        builder.Property(t => t.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(t => t.LogoUrl).HasMaxLength(500);
        builder.Property(t => t.PrimaryColor).HasMaxLength(20);
        builder.Property(t => t.SidebarColor).HasMaxLength(20);

        // Soft-delete query filter is applied globally in ApplicationDbContext.ApplyGlobalQueryFilters
        // Do NOT add an explicit HasQueryFilter here — EF Core only supports one filter per entity
        // and the dynamic one in ApplicationDbContext already handles ISoftDeletable types.
    }
}
