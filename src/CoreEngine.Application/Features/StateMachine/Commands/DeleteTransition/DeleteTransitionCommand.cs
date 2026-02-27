using MediatR;

namespace CoreEngine.Application.Features.StateMachine.Commands.DeleteTransition;

public record DeleteTransitionCommand(Guid Id) : IRequest;
