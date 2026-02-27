using CoreEngine.API.Filters;
using CoreEngine.Application.Common.Models;
using CoreEngine.Application.Features.AuditLogs.Queries.GetAuditLogs;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class AuditLogsController : BaseApiController
{
    [HttpGet]
    [RequirePermission(Permissions.AuditLogs.Read)]
    public async Task<ActionResult<PaginatedList<AuditLogDto>>> GetAll([FromQuery] GetAuditLogsQuery query)
        => Ok(await Mediator.Send(query));
}
