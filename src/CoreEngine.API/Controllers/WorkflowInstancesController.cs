using CoreEngine.API.Filters;
using CoreEngine.Application.Common.Models;
using CoreEngine.Application.Features.Workflows.Commands.CancelWorkflow;
using CoreEngine.Application.Features.Workflows.Commands.StartWorkflow;
using CoreEngine.Application.Features.Workflows.Queries.GetWorkflowInstances;
using CoreEngine.Application.Features.Workflows.Queries.GetWorkflowStatistics;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class WorkflowInstancesController : BaseApiController
{
    [HttpGet]
    [RequirePermission(Permissions.Workflows.View)]
    public async Task<ActionResult<PaginatedList<WorkflowInstanceDto>>> GetAll([FromQuery] GetWorkflowInstancesQuery query)
        => Ok(await Mediator.Send(query));

    [HttpGet("statistics")]
    [RequirePermission(Permissions.Workflows.View)]
    public async Task<ActionResult<WorkflowStatisticsDto>> GetStatistics()
        => Ok(await Mediator.Send(new GetWorkflowStatisticsQuery()));

    [HttpPost]
    [RequirePermission(Permissions.Workflows.Start)]
    public async Task<ActionResult<Guid>> Start(StartWorkflowCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id:guid}/cancel")]
    [RequirePermission(Permissions.Workflows.Cancel)]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await Mediator.Send(new CancelWorkflowCommand(id));
        return NoContent();
    }
}
