using MediatR;

namespace CoreEngine.Application.Features.Production.Commands.CreateProductionLog;

public record CreateProductionLogCommand(
    Guid MineSiteId,
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
    string? Notes
) : IRequest<Guid>;
