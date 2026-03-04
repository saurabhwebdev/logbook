using MediatR;

namespace CoreEngine.Application.Features.Shifts.Commands.CreateShiftDefinition;

public record CreateShiftDefinitionCommand(
    Guid MineSiteId,
    string Name,
    string? Code,
    string StartTime,
    string EndTime,
    int? ShiftOrder,
    string? Color,
    bool? IsActive
) : IRequest<Guid>;
