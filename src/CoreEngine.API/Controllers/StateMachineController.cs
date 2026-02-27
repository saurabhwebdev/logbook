using CoreEngine.API.Filters;
using CoreEngine.Application.Features.StateMachine.Commands.CreateState;
using CoreEngine.Application.Features.StateMachine.Commands.CreateTransition;
using CoreEngine.Application.Features.StateMachine.Commands.DeleteState;
using CoreEngine.Application.Features.StateMachine.Commands.DeleteTransition;
using CoreEngine.Application.Features.StateMachine.Commands.UpdateState;
using CoreEngine.Application.Features.StateMachine.Commands.UpdateTransition;
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

    [HttpPost("states")]
    [RequirePermission("StateMachine.Manage")]
    public async Task<ActionResult<Guid>> CreateState([FromBody] CreateStateCommand command)
        => Ok(await Mediator.Send(command));

    [HttpPut("states/{id}")]
    [RequirePermission("StateMachine.Manage")]
    public async Task<ActionResult> UpdateState(Guid id, [FromBody] UpdateStateRequest request)
    {
        await Mediator.Send(new UpdateStateCommand(
            id,
            request.StateName,
            request.IsInitial,
            request.IsFinal,
            request.Color,
            request.SortOrder
        ));
        return NoContent();
    }

    [HttpDelete("states/{id}")]
    [RequirePermission("StateMachine.Manage")]
    public async Task<ActionResult> DeleteState(Guid id)
    {
        await Mediator.Send(new DeleteStateCommand(id));
        return NoContent();
    }

    [HttpPost("transitions")]
    [RequirePermission("StateMachine.Manage")]
    public async Task<ActionResult<Guid>> CreateTransition([FromBody] CreateTransitionCommand command)
        => Ok(await Mediator.Send(command));

    [HttpPut("transitions/{id}")]
    [RequirePermission("StateMachine.Manage")]
    public async Task<ActionResult> UpdateTransition(Guid id, [FromBody] UpdateTransitionRequest request)
    {
        await Mediator.Send(new UpdateTransitionCommand(
            id,
            request.FromState,
            request.ToState,
            request.TriggerName,
            request.RequiredPermission,
            request.Description
        ));
        return NoContent();
    }

    [HttpDelete("transitions/{id}")]
    [RequirePermission("StateMachine.Manage")]
    public async Task<ActionResult> DeleteTransition(Guid id)
    {
        await Mediator.Send(new DeleteTransitionCommand(id));
        return NoContent();
    }
}

public record UpdateStateRequest(
    string StateName,
    bool IsInitial,
    bool IsFinal,
    string? Color,
    int SortOrder
);

public record UpdateTransitionRequest(
    string FromState,
    string ToState,
    string TriggerName,
    string? RequiredPermission,
    string? Description
);
