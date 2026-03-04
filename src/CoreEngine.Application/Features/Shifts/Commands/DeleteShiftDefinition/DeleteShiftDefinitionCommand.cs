using MediatR;

namespace CoreEngine.Application.Features.Shifts.Commands.DeleteShiftDefinition;

public record DeleteShiftDefinitionCommand(Guid Id) : IRequest<Unit>;
