using CoreEngine.API.Filters;
using CoreEngine.Application.Features.Blasting.Commands.CreateBlastEvent;
using CoreEngine.Application.Features.Blasting.Commands.CreateExplosiveUsage;
using CoreEngine.Application.Features.Blasting.Commands.DeleteBlastEvent;
using CoreEngine.Application.Features.Blasting.Commands.UpdateBlastEvent;
using CoreEngine.Application.Features.Blasting.DTOs;
using CoreEngine.Application.Features.Blasting.Queries.GetBlastEventById;
using CoreEngine.Application.Features.Blasting.Queries.GetBlastEvents;
using CoreEngine.Application.Features.Blasting.Queries.GetExplosiveUsages;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class BlastingController : BaseApiController
{
    [HttpGet]
    [RequirePermission(Permissions.Blasting.Read)]
    public async Task<ActionResult<IReadOnlyList<BlastEventDto>>> GetAll(
        [FromQuery] Guid? mineSiteId,
        [FromQuery] string? status)
        => Ok(await Mediator.Send(new GetBlastEventsQuery(mineSiteId, status)));

    [HttpGet("{id:guid}")]
    [RequirePermission(Permissions.Blasting.Read)]
    public async Task<ActionResult<BlastEventDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetBlastEventByIdQuery(id)));

    [HttpPost]
    [RequirePermission(Permissions.Blasting.Create)]
    public async Task<ActionResult<Guid>> Create(CreateBlastEventCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(Permissions.Blasting.Update)]
    public async Task<IActionResult> Update(Guid id, UpdateBlastEventCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(Permissions.Blasting.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteBlastEventCommand(id));
        return NoContent();
    }

    // Explosive Usages
    [HttpGet("{blastEventId:guid}/usages")]
    [RequirePermission(Permissions.Blasting.Read)]
    public async Task<ActionResult<IReadOnlyList<ExplosiveUsageDto>>> GetUsages(Guid blastEventId)
        => Ok(await Mediator.Send(new GetExplosiveUsagesQuery(blastEventId)));

    [HttpPost("{blastEventId:guid}/usages")]
    [RequirePermission(Permissions.Blasting.ManageExplosives)]
    public async Task<ActionResult<Guid>> CreateUsage(Guid blastEventId, CreateExplosiveUsageCommand command)
    {
        if (blastEventId != command.BlastEventId) return BadRequest("ID mismatch.");
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = blastEventId }, result);
    }
}
