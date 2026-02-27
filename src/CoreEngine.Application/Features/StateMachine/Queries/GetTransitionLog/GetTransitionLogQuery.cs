using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.StateMachine.Queries.GetTransitionLog;

public record TransitionLogDto(Guid Id, string FromState, string ToState, string TriggerName, string? PerformedBy, string? Comments, DateTime TransitionedAt);

public record GetTransitionLogQuery(string EntityType, string EntityId) : IRequest<List<TransitionLogDto>>;

public class GetTransitionLogQueryHandler : IRequestHandler<GetTransitionLogQuery, List<TransitionLogDto>>
{
    private readonly IApplicationDbContext _context;
    public GetTransitionLogQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<TransitionLogDto>> Handle(GetTransitionLogQuery request, CancellationToken ct)
    {
        return await _context.StateTransitionLogs
            .Where(l => l.EntityType == request.EntityType && l.EntityId == request.EntityId)
            .OrderByDescending(l => l.TransitionedAt)
            .Select(l => new TransitionLogDto(l.Id, l.FromState, l.ToState, l.TriggerName, l.PerformedBy, l.Comments, l.TransitionedAt))
            .ToListAsync(ct);
    }
}
