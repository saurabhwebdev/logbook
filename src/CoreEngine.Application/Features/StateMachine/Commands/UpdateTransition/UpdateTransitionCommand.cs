using MediatR;

namespace CoreEngine.Application.Features.StateMachine.Commands.UpdateTransition;

public record UpdateTransitionCommand(
    Guid Id,
    string FromState,
    string ToState,
    string TriggerName,
    string? RequiredPermission,
    string? Description
) : IRequest;
