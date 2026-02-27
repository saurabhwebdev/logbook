using CoreEngine.API.Filters;
using CoreEngine.Application.Common.Models;
using CoreEngine.Application.Features.Workflows.Commands.CompleteTask;
using CoreEngine.Application.Features.Workflows.Commands.ReassignTask;
using CoreEngine.Application.Features.Workflows.Queries.GetMyTasks;
using CoreEngine.Application.Features.Workflows.Queries.GetTaskById;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class WorkflowTasksController : BaseApiController
{
    [HttpGet("my-tasks")]
    [RequirePermission(Permissions.WorkflowTasks.View)]
    public async Task<ActionResult<PaginatedList<WorkflowTaskDto>>> GetMyTasks([FromQuery] GetMyTasksQuery query)
        => Ok(await Mediator.Send(query));

    [HttpGet("{id:guid}")]
    [RequirePermission(Permissions.WorkflowTasks.View)]
    public async Task<ActionResult<WorkflowTaskDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetTaskByIdQuery(id)));

    [HttpPost("{id:guid}/complete")]
    [RequirePermission(Permissions.WorkflowTasks.Complete)]
    public async Task<IActionResult> Complete(Guid id, [FromBody] CompleteTaskRequest request)
    {
        await Mediator.Send(new CompleteTaskCommand(id, request.Status, request.Comments));
        return NoContent();
    }

    [HttpPost("{id:guid}/reassign")]
    [RequirePermission(Permissions.WorkflowTasks.Reassign)]
    public async Task<IActionResult> Reassign(Guid id, [FromBody] ReassignTaskRequest request)
    {
        await Mediator.Send(new ReassignTaskCommand(id, request.NewAssigneeUserId));
        return NoContent();
    }
}

public record CompleteTaskRequest(string Status, string? Comments = null);
public record ReassignTaskRequest(Guid NewAssigneeUserId);
