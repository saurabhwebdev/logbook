using MediatR;

namespace CoreEngine.Application.Features.Shifts.Commands.AcknowledgeShiftHandover;

public record AcknowledgeShiftHandoverCommand(Guid Id) : IRequest<Unit>;
