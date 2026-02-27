using MediatR;

namespace CoreEngine.Application.Features.StateMachine.Commands.CreateTransition;

public record CreateTransitionCommand(
    string EntityType,
    string FromState,
    string ToState,
    string TriggerName,
    string? RequiredPermission,
    string? Description
) : IRequest<Guid>;
