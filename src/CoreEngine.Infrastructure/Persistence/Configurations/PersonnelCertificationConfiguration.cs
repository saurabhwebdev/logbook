using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreEngine.Infrastructure.Persistence.Configurations;

public class PersonnelCertificationConfiguration : IEntityTypeConfiguration<PersonnelCertification>
{
    public void Configure(EntityTypeBuilder<PersonnelCertification> builder)
    {
        builder.ToTable("PersonnelCertifications");

        builder.Property(e => e.CertificationName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.CertificateNumber).HasMaxLength(100);
        builder.Property(e => e.IssuingAuthority).HasMaxLength(200);
        builder.Property(e => e.Status).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Category).HasMaxLength(50);
        builder.Property(e => e.Notes).HasMaxLength(1000);

        builder.HasOne(e => e.Personnel).WithMany(p => p.Certifications).HasForeignKey(e => e.PersonnelId).OnDelete(DeleteBehavior.Restrict);
    }
}
