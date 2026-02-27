using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Interfaces;
using MediatR;

namespace CoreEngine.Application.Features.Notifications.Commands.MarkNotificationRead;

public record MarkNotificationReadCommand(Guid Id) : IRequest;

public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    public MarkNotificationReadCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(MarkNotificationReadCommand request, CancellationToken ct)
    {
        var notification = await _context.Notifications.FindAsync(new object[] { request.Id }, ct)
            ?? throw new NotFoundException("Notification", request.Id);
        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);
    }
}
