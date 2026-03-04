using CoreEngine.API.Filters;
using CoreEngine.Application.Features.StatutoryRegisters.Commands.AmendRegisterEntry;
using CoreEngine.Application.Features.StatutoryRegisters.Commands.CreateRegisterEntry;
using CoreEngine.Application.Features.StatutoryRegisters.DTOs;
using CoreEngine.Application.Features.StatutoryRegisters.Queries.GetRegisterEntries;
using CoreEngine.Application.Features.StatutoryRegisters.Queries.GetRegisterEntryById;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class RegisterEntriesController : BaseApiController
{
    [HttpGet("{statutoryRegisterId:guid}")]
    [RequirePermission(Permissions.RegisterEntries.Read)]
    public async Task<ActionResult<IReadOnlyList<RegisterEntryDto>>> GetByRegister(
        Guid statutoryRegisterId,
        [FromQuery] string? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
        => Ok(await Mediator.Send(new GetRegisterEntriesQuery(statutoryRegisterId, status, fromDate, toDate)));

    [HttpGet("detail/{id:guid}")]
    [RequirePermission(Permissions.RegisterEntries.Read)]
    public async Task<ActionResult<RegisterEntryDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetRegisterEntryByIdQuery(id)));

    [HttpPost]
    [RequirePermission(Permissions.RegisterEntries.Create)]
    public async Task<ActionResult<Guid>> Create(CreateRegisterEntryCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetByRegister), new { statutoryRegisterId = command.StatutoryRegisterId }, result);
    }

    [HttpPost("{id:guid}/amend")]
    [RequirePermission(Permissions.RegisterEntries.Amend)]
    public async Task<ActionResult<Guid>> Amend(Guid id, AmendRegisterEntryCommand command)
    {
        if (id != command.OriginalEntryId) return BadRequest("ID mismatch.");
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }
}
