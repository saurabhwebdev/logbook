namespace CoreEngine.Application.Features.Inspections.DTOs;

public record InspectionFindingDto(
    Guid Id,
    Guid InspectionId,
    string InspectionTitle,
    string FindingNumber,
    string Category,
    string Severity,
    string Description,
    string? Location,
    string? RecommendedAction,
    string? AssignedTo,
    DateTime? ActionDueDate,
    DateTime? ActionCompletedDate,
    string Status,
    string? ClosureNotes,
    DateTime CreatedAt);
