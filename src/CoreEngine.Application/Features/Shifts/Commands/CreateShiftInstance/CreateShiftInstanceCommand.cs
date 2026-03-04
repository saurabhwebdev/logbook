using MediatR;

namespace CoreEngine.Application.Features.Shifts.Commands.CreateShiftInstance;

public record CreateShiftInstanceCommand(
    Guid ShiftDefinitionId,
    Guid MineSiteId,
    string Date,
    string? SupervisorName,
    Guid? SupervisorId,
    string? Status,
    DateTime? ActualStartTime,
    DateTime? ActualEndTime,
    int? PersonnelCount,
    string? WeatherConditions,
    string? Notes
) : IRequest<Guid>;
