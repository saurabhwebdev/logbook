using CoreEngine.API.Filters;
using CoreEngine.Application.Features.Shifts.Commands.AcknowledgeShiftHandover;
using CoreEngine.Application.Features.Shifts.Commands.CreateShiftHandover;
using CoreEngine.Application.Features.Shifts.Commands.UpdateShiftHandover;
using CoreEngine.Application.Features.Shifts.DTOs;
using CoreEngine.Application.Features.Shifts.Queries.GetShiftHandovers;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class ShiftHandoversController : BaseApiController
{
    [HttpGet("{mineSiteId:guid}")]
    [RequirePermission(Permissions.ShiftHandovers.Read)]
    public async Task<ActionResult<IReadOnlyList<ShiftHandoverDto>>> GetByMineSite(
        Guid mineSiteId,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate)
        => Ok(await Mediator.Send(new GetShiftHandoversQuery(mineSiteId, fromDate, toDate)));

    [HttpPost]
    [RequirePermission(Permissions.ShiftHandovers.Create)]
    public async Task<ActionResult<Guid>> Create(CreateShiftHandoverCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetByMineSite), new { mineSiteId = command.MineSiteId }, result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(Permissions.ShiftHandovers.Update)]
    public async Task<IActionResult> Update(Guid id, UpdateShiftHandoverCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpPut("{id:guid}/acknowledge")]
    [RequirePermission(Permissions.ShiftHandovers.Update)]
    public async Task<IActionResult> Acknowledge(Guid id)
    {
        await Mediator.Send(new AcknowledgeShiftHandoverCommand(id));
        return NoContent();
    }
}
