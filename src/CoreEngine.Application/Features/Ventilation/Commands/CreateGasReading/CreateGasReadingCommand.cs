using MediatR;

namespace CoreEngine.Application.Features.Ventilation.Commands.CreateGasReading;

public record CreateGasReadingCommand(
    Guid MineSiteId,
    Guid? MineAreaId,
    string GasType,
    decimal Concentration,
    string Unit,
    decimal? ThresholdTWA,
    decimal? ThresholdSTEL,
    decimal? ThresholdCeiling,
    bool IsExceedance,
    string LocationDescription,
    DateTime ReadingDateTime,
    string RecordedBy,
    string? InstrumentId,
    DateTime? CalibrationDate,
    string? ActionTaken,
    string? Notes
) : IRequest<Guid>;
