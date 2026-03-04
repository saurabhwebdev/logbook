using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class MineSiteConfiguration : IEntityTypeConfiguration<MineSite>
{
    public void Configure(EntityTypeBuilder<MineSite> builder)
    {
        builder.ToTable("MineSites");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(m => m.Code)
            .HasMaxLength(50);

        builder.Property(m => m.MineType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.Jurisdiction)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.JurisdictionDetails)
            .HasMaxLength(500);

        builder.Property(m => m.Address)
            .HasMaxLength(500);

        builder.Property(m => m.Country)
            .HasMaxLength(100);

        builder.Property(m => m.State)
            .HasMaxLength(100);

        builder.Property(m => m.MineralsMined)
            .HasMaxLength(500);

        builder.Property(m => m.OperatingCompany)
            .HasMaxLength(300);

        builder.Property(m => m.MiningLicenseNumber)
            .HasMaxLength(100);

        builder.Property(m => m.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Active");

        builder.Property(m => m.EmergencyContactName)
            .HasMaxLength(200);

        builder.Property(m => m.EmergencyContactPhone)
            .HasMaxLength(50);

        builder.Property(m => m.NearestHospital)
            .HasMaxLength(300);

        builder.Property(m => m.NearestHospitalPhone)
            .HasMaxLength(50);

        builder.Property(m => m.UnitSystem)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Metric");

        builder.Property(m => m.TimeZone)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("UTC");

        builder.Property(m => m.ShiftsPerDay)
            .HasDefaultValue(3);

        builder.Property(m => m.ShiftPattern)
            .HasMaxLength(200);

        // FK to Tenant
        builder.HasOne(m => m.Tenant)
            .WithMany()
            .HasForeignKey(m => m.TenantId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // Navigation to MineAreas
        builder.HasMany(m => m.MineAreas)
            .WithOne(a => a.MineSite)
            .HasForeignKey(a => a.MineSiteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique code per tenant
        builder.HasIndex(m => new { m.TenantId, m.Code })
            .IsUnique()
            .HasFilter("[Code] IS NOT NULL AND [IsDeleted] = 0");
    }
}
