using CoreEngine.API.Filters;
using CoreEngine.Application.Features.MineSites.Commands.CreateMineArea;
using CoreEngine.Application.Features.MineSites.Commands.DeleteMineArea;
using CoreEngine.Application.Features.MineSites.Commands.UpdateMineArea;
using CoreEngine.Application.Features.MineSites.DTOs;
using CoreEngine.Application.Features.MineSites.Queries.GetMineAreas;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class MineAreasController : BaseApiController
{
    [HttpGet("{mineSiteId:guid}")]
    [RequirePermission(Permissions.MineAreas.Read)]
    public async Task<ActionResult<IReadOnlyList<MineAreaDto>>> GetByMineSite(Guid mineSiteId)
        => Ok(await Mediator.Send(new GetMineAreasQuery(mineSiteId)));

    [HttpPost]
    [RequirePermission(Permissions.MineAreas.Create)]
    public async Task<ActionResult<Guid>> Create(CreateMineAreaCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetByMineSite), new { mineSiteId = command.MineSiteId }, result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(Permissions.MineAreas.Update)]
    public async Task<IActionResult> Update(Guid id, UpdateMineAreaCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(Permissions.MineAreas.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteMineAreaCommand(id));
        return NoContent();
    }
}
