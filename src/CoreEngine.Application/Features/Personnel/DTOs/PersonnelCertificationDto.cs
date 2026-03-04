namespace CoreEngine.Application.Features.Personnel.DTOs;

public record PersonnelCertificationDto(
    Guid Id, Guid PersonnelId, string PersonnelName,
    string CertificationName, string? CertificateNumber, string? IssuingAuthority,
    DateTime IssueDate, DateTime? ExpiryDate, string Status, string? Category,
    string? Notes, DateTime CreatedAt);
