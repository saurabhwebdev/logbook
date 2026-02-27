using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;

namespace CoreEngine.Application.Features.Notifications.Commands.SendNotification;

public record SendNotificationCommand(Guid RecipientUserId, string Title, string Message, string Type = "Info", string? Link = null) : IRequest<Guid>;

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public SendNotificationCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(SendNotificationCommand request, CancellationToken ct)
    {
        var notification = new Notification
        {
            RecipientUserId = request.RecipientUserId,
            Title = request.Title,
            Message = request.Message,
            Type = request.Type,
            Link = request.Link,
        };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(ct);
        return notification.Id;
    }
}
