using CoreEngine.API.Filters;
using CoreEngine.Application.Features.WorkPermits.Commands.CreateWorkPermit;
using CoreEngine.Application.Features.WorkPermits.Commands.DeleteWorkPermit;
using CoreEngine.Application.Features.WorkPermits.Commands.UpdateWorkPermit;
using CoreEngine.Application.Features.WorkPermits.DTOs;
using CoreEngine.Application.Features.WorkPermits.Queries.GetWorkPermitById;
using CoreEngine.Application.Features.WorkPermits.Queries.GetWorkPermits;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class WorkPermitsController : BaseApiController
{
    [HttpGet]
    [RequirePermission(Permissions.WorkPermits.Read)]
    public async Task<ActionResult<IReadOnlyList<WorkPermitDto>>> GetAll(
        [FromQuery] Guid? mineSiteId,
        [FromQuery] string? status,
        [FromQuery] string? permitType)
        => Ok(await Mediator.Send(new GetWorkPermitsQuery(mineSiteId, status, permitType)));

    [HttpGet("{id:guid}")]
    [RequirePermission(Permissions.WorkPermits.Read)]
    public async Task<ActionResult<WorkPermitDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetWorkPermitByIdQuery(id)));

    [HttpPost]
    [RequirePermission(Permissions.WorkPermits.Create)]
    public async Task<ActionResult<Guid>> Create(CreateWorkPermitCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(Permissions.WorkPermits.Update)]
    public async Task<IActionResult> Update(Guid id, UpdateWorkPermitCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(Permissions.WorkPermits.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteWorkPermitCommand(id));
        return NoContent();
    }
}
