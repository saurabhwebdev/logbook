using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Notifications.Queries.GetNotifications;

public record NotificationDto(Guid Id, string Title, string Message, string Type, string? Link, bool IsRead, DateTime CreatedAt);

public record GetNotificationsQuery(bool UnreadOnly = false) : IRequest<List<NotificationDto>>;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, List<NotificationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    public GetNotificationsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken ct)
    {
        var userId = Guid.Parse(_currentUser.UserId ?? throw new UnauthorizedAccessException());
        var query = _context.Notifications.Where(n => n.RecipientUserId == userId);
        if (request.UnreadOnly) query = query.Where(n => !n.IsRead);
        return await query.OrderByDescending(n => n.CreatedAt).Take(50)
            .Select(n => new NotificationDto(n.Id, n.Title, n.Message, n.Type, n.Link, n.IsRead, n.CreatedAt))
            .ToListAsync(ct);
    }
}
