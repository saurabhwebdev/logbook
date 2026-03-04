namespace CoreEngine.Application.Features.Production.DTOs;

public record ProductionLogDto(
    Guid Id,
    Guid MineSiteId,
    string MineSiteName,
    Guid? MineAreaId,
    string? MineAreaName,
    Guid? ShiftInstanceId,
    string LogNumber,
    DateTime Date,
    string? ShiftName,
    string Material,
    string? SourceLocation,
    string? DestinationLocation,
    decimal QuantityTonnes,
    decimal? QuantityBCM,
    string? EquipmentUsed,
    string? OperatorName,
    double? HaulingDistance,
    int? LoadCount,
    string Status,
    string? Notes,
    string? VerifiedBy,
    DateTime? VerifiedAt,
    DateTime CreatedAt
);
