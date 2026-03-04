namespace CoreEngine.Application.Features.MineSites.DTOs;

public record MineAreaDto(
    Guid Id,
    Guid MineSiteId,
    string MineSiteName,
    string Name,
    string? Code,
    string AreaType,
    string? Description,
    double? Elevation,
    bool IsActive,
    Guid? ParentAreaId,
    string? ParentAreaName,
    int SortOrder,
    DateTime CreatedAt,
    int ChildAreaCount
);
