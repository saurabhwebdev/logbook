using CoreEngine.API.Filters;
using CoreEngine.Application.Features.DemoTasks.Commands.CreateDemoTask;
using CoreEngine.Application.Features.DemoTasks.Commands.DeleteDemoTask;
using CoreEngine.Application.Features.DemoTasks.Commands.TransitionDemoTask;
using CoreEngine.Application.Features.DemoTasks.Queries.GetDemoTasks;
using CoreEngine.Application.Features.StateMachine.Queries.GetStateDefinitions;
using CoreEngine.Application.Features.StateMachine.Queries.GetTransitionLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

[Authorize]
public class DemoTasksController : BaseApiController
{
    [HttpGet]
    [RequirePermission("DemoTask.Read")]
    public async Task<ActionResult<List<DemoTaskDto>>> GetAll()
        => Ok(await Mediator.Send(new GetDemoTasksQuery()));

    [HttpPost]
    [RequirePermission("DemoTask.Create")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateDemoTaskCommand command)
        => Ok(await Mediator.Send(command));

    [HttpPut("{id:guid}/transition")]
    [RequirePermission("DemoTask.Transition")]
    public async Task<ActionResult> Transition(Guid id, [FromBody] TransitionRequest request)
    {
        await Mediator.Send(new TransitionDemoTaskCommand(id, request.TriggerName, request.Comments));
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission("DemoTask.Delete")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteDemoTaskCommand(id));
        return NoContent();
    }

    [HttpGet("{id:guid}/history")]
    [RequirePermission("DemoTask.Read")]
    public async Task<ActionResult<List<TransitionLogDto>>> GetHistory(Guid id)
        => Ok(await Mediator.Send(new GetTransitionLogQuery("Task", id.ToString())));

    [HttpGet("states")]
    [RequirePermission("DemoTask.Read")]
    public async Task<ActionResult<StateDefinitionsResponse>> GetStates()
        => Ok(await Mediator.Send(new GetStateDefinitionsQuery("Task")));
}

public record TransitionRequest(string TriggerName, string? Comments);
