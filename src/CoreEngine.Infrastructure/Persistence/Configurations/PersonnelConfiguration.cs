using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class PersonnelConfiguration : IEntityTypeConfiguration<Personnel>
{
    public void Configure(EntityTypeBuilder<Personnel> builder)
    {
        builder.ToTable("Personnel");

        builder.Property(e => e.EmployeeNumber).HasMaxLength(50).IsRequired();
        builder.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.LastName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.MiddleName).HasMaxLength(100);
        builder.Property(e => e.Role).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Department).HasMaxLength(100);
        builder.Property(e => e.Designation).HasMaxLength(100);
        builder.Property(e => e.EmploymentType).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Status).HasMaxLength(30).IsRequired();
        builder.Property(e => e.ContactPhone).HasMaxLength(20);
        builder.Property(e => e.ContactEmail).HasMaxLength(200);
        builder.Property(e => e.EmergencyContactName).HasMaxLength(200);
        builder.Property(e => e.EmergencyContactPhone).HasMaxLength(20);
        builder.Property(e => e.BloodGroup).HasMaxLength(10);
        builder.Property(e => e.MedicalFitnessCertificate).HasMaxLength(200);
        builder.Property(e => e.Notes).HasMaxLength(2000);

        builder.HasOne(e => e.MineSite).WithMany().HasForeignKey(e => e.MineSiteId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.EmployeeNumber }).IsUnique().HasFilter("IsDeleted = 0");
    }
}
