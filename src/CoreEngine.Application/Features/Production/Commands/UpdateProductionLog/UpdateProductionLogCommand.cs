using MediatR;

namespace CoreEngine.Application.Features.Production.Commands.UpdateProductionLog;

public record UpdateProductionLogCommand(
    Guid Id,
    Guid? MineAreaId,
    Guid? ShiftInstanceId,
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
    DateTime? VerifiedAt
) : IRequest;
