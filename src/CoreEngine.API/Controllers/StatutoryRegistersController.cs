using CoreEngine.API.Filters;
using CoreEngine.Application.Features.StatutoryRegisters.Commands.CreateStatutoryRegister;
using CoreEngine.Application.Features.StatutoryRegisters.Commands.DeleteStatutoryRegister;
using CoreEngine.Application.Features.StatutoryRegisters.Commands.UpdateStatutoryRegister;
using CoreEngine.Application.Features.StatutoryRegisters.DTOs;
using CoreEngine.Application.Features.StatutoryRegisters.Queries.GetStatutoryRegisterById;
using CoreEngine.Application.Features.StatutoryRegisters.Queries.GetStatutoryRegisters;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class StatutoryRegistersController : BaseApiController
{
    [HttpGet("{mineSiteId:guid}")]
    [RequirePermission(Permissions.StatutoryRegisters.Read)]
    public async Task<ActionResult<IReadOnlyList<StatutoryRegisterDto>>> GetByMineSite(Guid mineSiteId)
        => Ok(await Mediator.Send(new GetStatutoryRegistersQuery(mineSiteId)));

    [HttpGet("detail/{id:guid}")]
    [RequirePermission(Permissions.StatutoryRegisters.Read)]
    public async Task<ActionResult<StatutoryRegisterDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetStatutoryRegisterByIdQuery(id)));

    [HttpPost]
    [RequirePermission(Permissions.StatutoryRegisters.Create)]
    public async Task<ActionResult<Guid>> Create(CreateStatutoryRegisterCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetByMineSite), new { mineSiteId = command.MineSiteId }, result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(Permissions.StatutoryRegisters.Update)]
    public async Task<IActionResult> Update(Guid id, UpdateStatutoryRegisterCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(Permissions.StatutoryRegisters.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteStatutoryRegisterCommand(id));
        return NoContent();
    }
}
