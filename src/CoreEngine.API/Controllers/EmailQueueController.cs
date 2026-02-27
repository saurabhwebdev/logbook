using CoreEngine.API.Filters;
using CoreEngine.Application.Features.Emails.Commands.QueueEmail;
using CoreEngine.Application.Features.Emails.Commands.SendEmail;
using CoreEngine.Application.Features.Emails.Queries.GetEmailQueue;
using CoreEngine.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

[Authorize]
public class EmailQueueController : BaseApiController
{
    [HttpGet]
    [RequirePermission("Email.Read")]
    public async Task<ActionResult<List<EmailQueueDto>>> GetQueue([FromQuery] EmailStatus? status = null)
        => Ok(await Mediator.Send(new GetEmailQueueQuery(status)));

    [HttpPost("send")]
    [RequirePermission("Email.Send")]
    public async Task<ActionResult<bool>> Send([FromBody] SendEmailCommand command)
        => Ok(await Mediator.Send(command));

    [HttpPost("queue")]
    [RequirePermission("Email.Send")]
    public async Task<ActionResult> Queue([FromBody] QueueEmailCommand command)
    {
        await Mediator.Send(command);
        return Ok(new { message = "Email queued successfully" });
    }
}
