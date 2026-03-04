using CoreEngine.API.Filters;
using CoreEngine.Application.Features.Shifts.Commands.CreateShiftInstance;
using CoreEngine.Application.Features.Shifts.Commands.DeleteShiftInstance;
using CoreEngine.Application.Features.Shifts.Commands.UpdateShiftInstance;
using CoreEngine.Application.Features.Shifts.DTOs;
using CoreEngine.Application.Features.Shifts.Queries.GetShiftInstanceById;
using CoreEngine.Application.Features.Shifts.Queries.GetShiftInstances;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class ShiftInstancesController : BaseApiController
{
    [HttpGet("{mineSiteId:guid}")]
    [RequirePermission(Permissions.ShiftInstances.Read)]
    public async Task<ActionResult<IReadOnlyList<ShiftInstanceDto>>> GetByMineSite(
        Guid mineSiteId,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate)
        => Ok(await Mediator.Send(new GetShiftInstancesQuery(mineSiteId, fromDate, toDate)));

    [HttpGet("detail/{id:guid}")]
    [RequirePermission(Permissions.ShiftInstances.Read)]
    public async Task<ActionResult<ShiftInstanceDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetShiftInstanceByIdQuery(id)));

    [HttpPost]
    [RequirePermission(Permissions.ShiftInstances.Create)]
    public async Task<ActionResult<Guid>> Create(CreateShiftInstanceCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(Permissions.ShiftInstances.Update)]
    public async Task<IActionResult> Update(Guid id, UpdateShiftInstanceCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(Permissions.ShiftInstances.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteShiftInstanceCommand(id));
        return NoContent();
    }
}
