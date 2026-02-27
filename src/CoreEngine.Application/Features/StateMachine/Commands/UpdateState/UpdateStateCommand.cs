using MediatR;

namespace CoreEngine.Application.Features.StateMachine.Commands.UpdateState;

public record UpdateStateCommand(
    Guid Id,
    string StateName,
    bool IsInitial,
    bool IsFinal,
    string? Color,
    int SortOrder
) : IRequest;
