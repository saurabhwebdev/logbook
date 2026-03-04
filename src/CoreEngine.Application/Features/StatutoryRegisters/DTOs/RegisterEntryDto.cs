namespace CoreEngine.Application.Features.StatutoryRegisters.DTOs;

public record RegisterEntryDto(
    Guid Id,
    Guid StatutoryRegisterId,
    string RegisterName,
    Guid MineSiteId,
    string MineSiteName,
    int EntryNumber,
    DateTime EntryDate,
    Guid? ShiftInstanceId,
    Guid? MineAreaId,
    string? MineAreaName,
    string Subject,
    string Details,
    string ReportedBy,
    string? WitnessName,
    string? ActionTaken,
    DateTime? ActionDueDate,
    DateTime? ActionCompletedDate,
    string Status,
    Guid? AmendmentOfEntryId,
    string? AmendmentReason,
    DateTime CreatedAt,
    int AmendmentCount
);
