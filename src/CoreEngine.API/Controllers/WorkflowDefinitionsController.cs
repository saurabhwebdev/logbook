using CoreEngine.API.Filters;
using CoreEngine.Application.Common.Models;
using CoreEngine.Application.Features.Workflows.Commands.CreateWorkflowDefinition;
using CoreEngine.Application.Features.Workflows.Queries.GetWorkflowDefinitions;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class WorkflowDefinitionsController : BaseApiController
{
    [HttpGet]
    [RequirePermission(Permissions.WorkflowDefinitions.Read)]
    public async Task<ActionResult<PaginatedList<WorkflowDefinitionDto>>> GetAll([FromQuery] GetWorkflowDefinitionsQuery query)
        => Ok(await Mediator.Send(query));

    [HttpPost]
    [RequirePermission(Permissions.WorkflowDefinitions.Create)]
    public async Task<ActionResult<Guid>> Create(CreateWorkflowDefinitionCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}
