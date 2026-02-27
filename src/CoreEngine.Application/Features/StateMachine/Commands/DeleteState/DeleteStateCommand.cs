using MediatR;

namespace CoreEngine.Application.Features.StateMachine.Commands.DeleteState;

public record DeleteStateCommand(Guid Id) : IRequest;
