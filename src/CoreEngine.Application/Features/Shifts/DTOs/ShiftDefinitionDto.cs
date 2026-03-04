namespace CoreEngine.Application.Features.Shifts.DTOs;

public record ShiftDefinitionDto(
    Guid Id,
    Guid MineSiteId,
    string MineSiteName,
    string Name,
    string? Code,
    string StartTime,
    string EndTime,
    int ShiftOrder,
    string? Color,
    bool IsActive,
    DateTime CreatedAt,
    int InstanceCount
);
