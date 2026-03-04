using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class PersonnelCertification : TenantScopedEntity
{
    public Guid PersonnelId { get; set; }
    public string CertificationName { get; set; } = default!;
    public string? CertificateNumber { get; set; }
    public string? IssuingAuthority { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Status { get; set; } = "Valid"; // Valid, Expired, Revoked, Pending
    public string? Category { get; set; } // Safety, Blasting, Electrical, FirstAid, Equipment, Statutory, Other
    public string? Notes { get; set; }

    // Navigation
    public Personnel Personnel { get; set; } = default!;
}
