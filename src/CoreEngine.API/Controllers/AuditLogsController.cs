using CoreEngine.API.Filters;
using CoreEngine.Application.Common.Models;
using CoreEngine.Application.Features.AuditLogs.Queries.GetAuditLogs;
using CoreEngine.Application.Features.AuditLogs.Queries.GetUserActivity;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class AuditLogsController : BaseApiController
{
    [HttpGet]
    [RequirePermission(Permissions.AuditLogs.Read)]
    public async Task<ActionResult<PaginatedList<AuditLogDto>>> GetAll([FromQuery] GetAuditLogsQuery query)
        => Ok(await Mediator.Send(query));

    [HttpGet("my-activity")]
    public async Task<ActionResult<List<UserActivityDto>>> GetMyActivity([FromQuery] int limit = 50)
        => Ok(await Mediator.Send(new GetUserActivityQuery(limit)));
}
