using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Notifications.Commands.MarkAllNotificationsRead;

public record MarkAllNotificationsReadCommand : IRequest<int>;

public class MarkAllNotificationsReadCommandHandler : IRequestHandler<MarkAllNotificationsReadCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    public MarkAllNotificationsReadCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(MarkAllNotificationsReadCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(_currentUser.UserId ?? throw new UnauthorizedAccessException());
        var unread = await _context.Notifications
            .Where(n => n.RecipientUserId == userId && !n.IsRead)
            .ToListAsync(ct);
        foreach (var n in unread) { n.IsRead = true; n.ReadAt = DateTime.UtcNow; }
        await _context.SaveChangesAsync(ct);
        return unread.Count;
    }
}
