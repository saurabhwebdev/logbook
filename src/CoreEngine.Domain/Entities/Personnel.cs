using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class Personnel : TenantScopedEntity
{
    public Guid MineSiteId { get; set; }
    public string EmployeeNumber { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? MiddleName { get; set; }
    public string Role { get; set; } = default!; // Miner, Operator, Supervisor, Engineer, Geologist, Electrician, Mechanic, Safety, Blaster, Manager, Contractor, Other
    public string? Department { get; set; }
    public string? Designation { get; set; }
    public string EmploymentType { get; set; } = "Permanent"; // Permanent, Contract, Casual, Apprentice
    public DateTime DateOfJoining { get; set; }
    public DateTime? DateOfLeaving { get; set; }
    public string Status { get; set; } = "Active"; // Active, OnLeave, Suspended, Terminated, Retired
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? BloodGroup { get; set; }
    public string? MedicalFitnessCertificate { get; set; }
    public DateTime? MedicalFitnessExpiry { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public MineSite MineSite { get; set; } = default!;
    public ICollection<PersonnelCertification> Certifications { get; set; } = new List<PersonnelCertification>();
}
