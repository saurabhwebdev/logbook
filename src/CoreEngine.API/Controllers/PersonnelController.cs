using CoreEngine.API.Filters;
using CoreEngine.Application.Features.Personnel.Commands.CreateCertification;
using CoreEngine.Application.Features.Personnel.Commands.CreatePersonnel;
using CoreEngine.Application.Features.Personnel.Commands.DeletePersonnel;
using CoreEngine.Application.Features.Personnel.Commands.UpdatePersonnel;
using CoreEngine.Application.Features.Personnel.DTOs;
using CoreEngine.Application.Features.Personnel.Queries.GetCertifications;
using CoreEngine.Application.Features.Personnel.Queries.GetPersonnel;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class PersonnelController : BaseApiController
{
    [HttpGet]
    [RequirePermission(Permissions.PersonnelMgmt.Read)]
    public async Task<ActionResult<IReadOnlyList<PersonnelDto>>> GetAll(
        [FromQuery] Guid? mineSiteId, [FromQuery] string? status, [FromQuery] string? role)
        => Ok(await Mediator.Send(new GetPersonnelQuery(mineSiteId, status, role)));

    [HttpPost]
    [RequirePermission(Permissions.PersonnelMgmt.Create)]
    public async Task<ActionResult<Guid>> Create(CreatePersonnelCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), new { id = result }, result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(Permissions.PersonnelMgmt.Update)]
    public async Task<IActionResult> Update(Guid id, UpdatePersonnelCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(Permissions.PersonnelMgmt.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeletePersonnelCommand(id));
        return NoContent();
    }

    [HttpGet("{personnelId:guid}/certifications")]
    [RequirePermission(Permissions.PersonnelMgmt.Read)]
    public async Task<ActionResult<IReadOnlyList<PersonnelCertificationDto>>> GetCertifications(Guid personnelId)
        => Ok(await Mediator.Send(new GetCertificationsQuery(personnelId)));

    [HttpPost("{personnelId:guid}/certifications")]
    [RequirePermission(Permissions.PersonnelMgmt.ManageCertifications)]
    public async Task<ActionResult<Guid>> CreateCertification(Guid personnelId, CreateCertificationCommand command)
    {
        if (personnelId != command.PersonnelId) return BadRequest("ID mismatch.");
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), new { id = personnelId }, result);
    }
}
