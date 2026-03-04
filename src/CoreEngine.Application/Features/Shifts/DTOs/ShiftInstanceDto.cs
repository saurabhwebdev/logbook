namespace CoreEngine.Application.Features.Shifts.DTOs;

public record ShiftInstanceDto(
    Guid Id,
    Guid ShiftDefinitionId,
    string ShiftDefinitionName,
    Guid MineSiteId,
    string MineSiteName,
    string Date,
    string? SupervisorName,
    Guid? SupervisorId,
    string Status,
    DateTime? ActualStartTime,
    DateTime? ActualEndTime,
    int? PersonnelCount,
    string? WeatherConditions,
    string? Notes,
    DateTime CreatedAt,
    int HandoverCount
);
