using MediatR;

namespace CoreEngine.Application.Features.Shifts.Commands.DeleteShiftInstance;

public record DeleteShiftInstanceCommand(Guid Id) : IRequest<Unit>;
