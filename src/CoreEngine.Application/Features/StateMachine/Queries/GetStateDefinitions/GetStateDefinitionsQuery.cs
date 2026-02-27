using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.StateMachine.Queries.GetStateDefinitions;

public record StateDefinitionDto(Guid Id, string EntityType, string StateName, bool IsInitial, bool IsFinal, string? Color, int SortOrder);
public record StateTransitionDefinitionDto(Guid Id, string EntityType, string FromState, string ToState, string TriggerName, string? RequiredPermission, string? Description);

public record GetStateDefinitionsQuery(string EntityType) : IRequest<StateDefinitionsResponse>;

public record StateDefinitionsResponse(List<StateDefinitionDto> States, List<StateTransitionDefinitionDto> Transitions);

public class GetStateDefinitionsQueryHandler : IRequestHandler<GetStateDefinitionsQuery, StateDefinitionsResponse>
{
    private readonly IApplicationDbContext _context;
    public GetStateDefinitionsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<StateDefinitionsResponse> Handle(GetStateDefinitionsQuery request, CancellationToken ct)
    {
        var states = await _context.StateDefinitions
            .Where(s => s.EntityType == request.EntityType)
            .OrderBy(s => s.SortOrder)
            .Select(s => new StateDefinitionDto(s.Id, s.EntityType, s.StateName, s.IsInitial, s.IsFinal, s.Color, s.SortOrder))
            .ToListAsync(ct);

        var transitions = await _context.StateTransitionDefinitions
            .Where(t => t.EntityType == request.EntityType)
            .Select(t => new StateTransitionDefinitionDto(t.Id, t.EntityType, t.FromState, t.ToState, t.TriggerName, t.RequiredPermission, t.Description))
            .ToListAsync(ct);

        return new StateDefinitionsResponse(states, transitions);
    }
}
