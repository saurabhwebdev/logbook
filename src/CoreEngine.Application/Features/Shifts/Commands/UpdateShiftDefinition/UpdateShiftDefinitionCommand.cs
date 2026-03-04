using MediatR;

namespace CoreEngine.Application.Features.Shifts.Commands.UpdateShiftDefinition;

public record UpdateShiftDefinitionCommand(
    Guid Id,
    string Name,
    string? Code,
    string StartTime,
    string EndTime,
    int ShiftOrder,
    string? Color,
    bool IsActive
) : IRequest<Unit>;
