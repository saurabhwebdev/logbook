using CoreEngine.API.Filters;
using CoreEngine.Application.Features.Notifications.Commands.MarkAllNotificationsRead;
using CoreEngine.Application.Features.Notifications.Commands.MarkNotificationRead;
using CoreEngine.Application.Features.Notifications.Commands.SendNotification;
using CoreEngine.Application.Features.Notifications.Queries.GetNotifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

[Authorize]
public class NotificationsController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<NotificationDto>>> GetMy([FromQuery] bool unreadOnly = false)
        => Ok(await Mediator.Send(new GetNotificationsQuery(unreadOnly)));

    [HttpPut("{id:guid}/read")]
    public async Task<ActionResult> MarkRead(Guid id)
    {
        await Mediator.Send(new MarkNotificationReadCommand(id));
        return NoContent();
    }

    [HttpPut("read-all")]
    public async Task<ActionResult<int>> MarkAllRead()
        => Ok(await Mediator.Send(new MarkAllNotificationsReadCommand()));

    [HttpPost]
    [RequirePermission("Notification.Send")]
    public async Task<ActionResult<Guid>> Send([FromBody] SendNotificationCommand command)
        => Ok(await Mediator.Send(command));
}
