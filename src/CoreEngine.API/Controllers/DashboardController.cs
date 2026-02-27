using CoreEngine.Application.Features.Dashboard.Queries.GetDashboardStats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

[Authorize]
public class DashboardController : BaseApiController
{
    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetStats()
        => Ok(await Mediator.Send(new GetDashboardStatsQuery()));
}
