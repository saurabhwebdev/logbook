using MediatR;

namespace CoreEngine.Application.Features.Ventilation.Commands.UpdateGasReading;

public record UpdateGasReadingCommand(
    Guid Id,
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
    string Status,
    string? Notes
) : IRequest;
