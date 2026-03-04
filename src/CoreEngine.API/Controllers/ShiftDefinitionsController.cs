using CoreEngine.API.Filters;
using CoreEngine.Application.Features.Shifts.Commands.CreateShiftDefinition;
using CoreEngine.Application.Features.Shifts.Commands.DeleteShiftDefinition;
using CoreEngine.Application.Features.Shifts.Commands.UpdateShiftDefinition;
using CoreEngine.Application.Features.Shifts.DTOs;
using CoreEngine.Application.Features.Shifts.Queries.GetShiftDefinitions;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class ShiftDefinitionsController : BaseApiController
{
    [HttpGet("{mineSiteId:guid}")]
    [RequirePermission(Permissions.ShiftDefinitions.Read)]
    public async Task<ActionResult<IReadOnlyList<ShiftDefinitionDto>>> GetByMineSite(Guid mineSiteId)
        => Ok(await Mediator.Send(new GetShiftDefinitionsQuery(mineSiteId)));

    [HttpPost]
    [RequirePermission(Permissions.ShiftDefinitions.Create)]
    public async Task<ActionResult<Guid>> Create(CreateShiftDefinitionCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetByMineSite), new { mineSiteId = command.MineSiteId }, result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(Permissions.ShiftDefinitions.Update)]
    public async Task<IActionResult> Update(Guid id, UpdateShiftDefinitionCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(Permissions.ShiftDefinitions.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteShiftDefinitionCommand(id));
        return NoContent();
    }
}
