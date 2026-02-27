using CoreEngine.API.Filters;
using CoreEngine.Application.Features.StateMachine.Queries.GetStateDefinitions;
using CoreEngine.Application.Features.StateMachine.Queries.GetTransitionLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

[Authorize]
public class StateMachineController : BaseApiController
{
    [HttpGet("{entityType}")]
    [RequirePermission("StateMachine.Read")]
    public async Task<ActionResult<StateDefinitionsResponse>> GetDefinitions(string entityType)
        => Ok(await Mediator.Send(new GetStateDefinitionsQuery(entityType)));

    [HttpGet("{entityType}/{entityId}/log")]
    [RequirePermission("StateMachine.Read")]
    public async Task<ActionResult<List<TransitionLogDto>>> GetTransitionLog(string entityType, string entityId)
        => Ok(await Mediator.Send(new GetTransitionLogQuery(entityType, entityId)));
}
