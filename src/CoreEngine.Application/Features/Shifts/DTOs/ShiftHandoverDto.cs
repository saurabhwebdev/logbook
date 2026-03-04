namespace CoreEngine.Application.Features.Shifts.DTOs;

public record ShiftHandoverDto(
    Guid Id,
    Guid OutgoingShiftInstanceId,
    string OutgoingShiftName,
    Guid? IncomingShiftInstanceId,
    string? IncomingShiftName,
    Guid MineSiteId,
    string MineSiteName,
    DateTime HandoverDateTime,
    string? SafetyIssues,
    string? OngoingOperations,
    string? PendingTasks,
    string? EquipmentStatus,
    string? EnvironmentalConditions,
    string? GeneralRemarks,
    string? HandedOverBy,
    string? ReceivedBy,
    string Status,
    DateTime? AcknowledgedAt,
    DateTime CreatedAt
);
