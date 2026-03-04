namespace CoreEngine.Application.Features.StatutoryRegisters.DTOs;

public record StatutoryRegisterDto(
    Guid Id,
    Guid MineSiteId,
    string MineSiteName,
    string Name,
    string? Code,
    string RegisterType,
    string? Description,
    string Jurisdiction,
    bool IsRequired,
    int RetentionYears,
    bool IsActive,
    int SortOrder,
    DateTime CreatedAt,
    int EntryCount
);
