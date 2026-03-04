namespace CoreEngine.Application.Features.Personnel.DTOs;

public record PersonnelDto(
    Guid Id, Guid MineSiteId, string MineSiteName,
    string EmployeeNumber, string FirstName, string LastName, string? MiddleName,
    string Role, string? Department, string? Designation,
    string EmploymentType, DateTime DateOfJoining, DateTime? DateOfLeaving,
    string Status, string? ContactPhone, string? ContactEmail,
    string? EmergencyContactName, string? EmergencyContactPhone,
    string? BloodGroup, string? MedicalFitnessCertificate, DateTime? MedicalFitnessExpiry,
    string? Notes, int CertificationCount, DateTime CreatedAt);
