using MediatR;

namespace CoreEngine.Application.Features.StateMachine.Commands.CreateState;

public record CreateStateCommand(
    string EntityType,
    string StateName,
    bool IsInitial,
    bool IsFinal,
    string? Color,
    int SortOrder
) : IRequest<Guid>;
