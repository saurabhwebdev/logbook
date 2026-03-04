using CoreEngine.API.Filters;
using CoreEngine.Application.Features.MineSites.Commands.CreateMineSite;
using CoreEngine.Application.Features.MineSites.Commands.DeleteMineSite;
using CoreEngine.Application.Features.MineSites.Commands.UpdateMineSite;
using CoreEngine.Application.Features.MineSites.DTOs;
using CoreEngine.Application.Features.MineSites.Queries.GetMineSiteById;
using CoreEngine.Application.Features.MineSites.Queries.GetMineSites;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class MineSitesController : BaseApiController
{
    [HttpGet]
    [RequirePermission(Permissions.MineSites.Read)]
    public async Task<ActionResult<IReadOnlyList<MineSiteDto>>> GetAll()
        => Ok(await Mediator.Send(new GetMineSitesQuery()));

    [HttpGet("{id:guid}")]
    [RequirePermission(Permissions.MineSites.Read)]
    public async Task<ActionResult<MineSiteDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetMineSiteByIdQuery(id)));

    [HttpPost]
    [RequirePermission(Permissions.MineSites.Create)]
    public async Task<ActionResult<Guid>> Create(CreateMineSiteCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(Permissions.MineSites.Update)]
    public async Task<IActionResult> Update(Guid id, UpdateMineSiteCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(Permissions.MineSites.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteMineSiteCommand(id));
        return NoContent();
    }
}
