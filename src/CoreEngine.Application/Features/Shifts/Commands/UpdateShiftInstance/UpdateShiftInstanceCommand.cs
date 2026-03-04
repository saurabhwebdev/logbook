using MediatR;

namespace CoreEngine.Application.Features.Shifts.Commands.UpdateShiftInstance;

public record UpdateShiftInstanceCommand(
    Guid Id,
    string? SupervisorName,
    Guid? SupervisorId,
    string Status,
    DateTime? ActualStartTime,
    DateTime? ActualEndTime,
    int? PersonnelCount,
    string? WeatherConditions,
    string? Notes
) : IRequest<Unit>;
