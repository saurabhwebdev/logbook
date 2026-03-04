namespace CoreEngine.Application.Features.Inspections.DTOs;

public record InspectionTemplateDto(
    Guid Id,
    string Name,
    string Code,
    string Category,
    string? Description,
    string? ChecklistJson,
    string Frequency,
    bool IsActive,
    int SortOrder,
    int InspectionCount,
    DateTime CreatedAt);
