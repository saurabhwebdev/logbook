using CoreEngine.API.Filters;
using CoreEngine.Application.Features.SafetyIncidents.Commands.CreateInvestigation;
using CoreEngine.Application.Features.SafetyIncidents.Commands.CreateSafetyIncident;
using CoreEngine.Application.Features.SafetyIncidents.Commands.DeleteSafetyIncident;
using CoreEngine.Application.Features.SafetyIncidents.Commands.UpdateSafetyIncident;
using CoreEngine.Application.Features.SafetyIncidents.DTOs;
using CoreEngine.Application.Features.SafetyIncidents.Queries.GetInvestigations;
using CoreEngine.Application.Features.SafetyIncidents.Queries.GetSafetyIncidentById;
using CoreEngine.Application.Features.SafetyIncidents.Queries.GetSafetyIncidents;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class SafetyIncidentsController : BaseApiController
{
    [HttpGet]
    [RequirePermission(Permissions.SafetyIncidents.Read)]
    public async Task<ActionResult<IReadOnlyList<SafetyIncidentDto>>> GetAll(
        [FromQuery] Guid? mineSiteId,
        [FromQuery] string? status,
        [FromQuery] string? severity)
        => Ok(await Mediator.Send(new GetSafetyIncidentsQuery(mineSiteId, status, severity)));

    [HttpGet("{id:guid}")]
    [RequirePermission(Permissions.SafetyIncidents.Read)]
    public async Task<ActionResult<SafetyIncidentDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetSafetyIncidentByIdQuery(id)));

    [HttpPost]
    [RequirePermission(Permissions.SafetyIncidents.Create)]
    public async Task<ActionResult<Guid>> Create(CreateSafetyIncidentCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(Permissions.SafetyIncidents.Update)]
    public async Task<IActionResult> Update(Guid id, UpdateSafetyIncidentCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(Permissions.SafetyIncidents.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteSafetyIncidentCommand(id));
        return NoContent();
    }

    // Investigations
    [HttpGet("{incidentId:guid}/investigations")]
    [RequirePermission(Permissions.SafetyIncidents.Read)]
    public async Task<ActionResult<IReadOnlyList<IncidentInvestigationDto>>> GetInvestigations(Guid incidentId)
        => Ok(await Mediator.Send(new GetInvestigationsQuery(incidentId)));

    [HttpPost("{incidentId:guid}/investigations")]
    [RequirePermission(Permissions.SafetyIncidents.Investigate)]
    public async Task<ActionResult<Guid>> CreateInvestigation(Guid incidentId, CreateInvestigationCommand command)
    {
        if (incidentId != command.SafetyIncidentId) return BadRequest("ID mismatch.");
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = incidentId }, result);
    }
}
