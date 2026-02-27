using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.AuditLogs.Queries.GetUserActivity;

public record GetUserActivityQuery(int Limit = 50) : IRequest<List<UserActivityDto>>;

public class GetUserActivityQueryHandler : IRequestHandler<GetUserActivityQuery, List<UserActivityDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetUserActivityQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<UserActivityDto>> Handle(GetUserActivityQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return new List<UserActivityDto>();
        }

        // Limit the range to avoid performance issues (20-50 activities)
        var limit = Math.Min(Math.Max(request.Limit, 20), 50);

        var activities = await _context.AuditLogs
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .Take(limit)
            .Select(a => new UserActivityDto(
                a.Id,
                a.Action,
                a.EntityName,
                a.EntityId,
                a.OldValues,
                a.NewValues,
                a.Timestamp
            ))
            .ToListAsync(cancellationToken);

        return activities;
    }
}
