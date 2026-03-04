using CoreEngine.API.Filters;
using CoreEngine.Application.Features.Environmental.Commands.CreateEnvironmentalIncident;
using CoreEngine.Application.Features.Environmental.Commands.CreateEnvironmentalReading;
using CoreEngine.Application.Features.Environmental.Commands.DeleteEnvironmentalReading;
using CoreEngine.Application.Features.Environmental.Commands.UpdateEnvironmentalIncident;
using CoreEngine.Application.Features.Environmental.Commands.UpdateEnvironmentalReading;
using CoreEngine.Application.Features.Environmental.DTOs;
using CoreEngine.Application.Features.Environmental.Queries.GetEnvironmentalIncidents;
using CoreEngine.Application.Features.Environmental.Queries.GetEnvironmentalReadings;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class EnvironmentalController : BaseApiController
{
    // ===== Readings =====

    [HttpGet("readings")]
    [RequirePermission(Permissions.Environmental.Read)]
    public async Task<ActionResult<IReadOnlyList<EnvironmentalReadingDto>>> GetReadings(
        [FromQuery] Guid? mineSiteId,
        [FromQuery] string? readingType,
        [FromQuery] string? status)
        => Ok(await Mediator.Send(new GetEnvironmentalReadingsQuery(mineSiteId, readingType, status)));

    [HttpPost("readings")]
    [RequirePermission(Permissions.Environmental.Create)]
    public async Task<ActionResult<Guid>> CreateReading(CreateEnvironmentalReadingCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetReadings), new { id = result }, result);
    }

    [HttpPut("readings/{id:guid}")]
    [RequirePermission(Permissions.Environmental.Update)]
    public async Task<IActionResult> UpdateReading(Guid id, UpdateEnvironmentalReadingCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("readings/{id:guid}")]
    [RequirePermission(Permissions.Environmental.Delete)]
    public async Task<IActionResult> DeleteReading(Guid id)
    {
        await Mediator.Send(new DeleteEnvironmentalReadingCommand(id));
        return NoContent();
    }

    // ===== Incidents =====

    [HttpGet("incidents")]
    [RequirePermission(Permissions.Environmental.Read)]
    public async Task<ActionResult<IReadOnlyList<EnvironmentalIncidentDto>>> GetIncidents(
        [FromQuery] Guid? mineSiteId,
        [FromQuery] string? status)
        => Ok(await Mediator.Send(new GetEnvironmentalIncidentsQuery(mineSiteId, status)));

    [HttpPost("incidents")]
    [RequirePermission(Permissions.Environmental.ManageIncidents)]
    public async Task<ActionResult<Guid>> CreateIncident(CreateEnvironmentalIncidentCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetIncidents), new { id = result }, result);
    }

    [HttpPut("incidents/{id:guid}")]
    [RequirePermission(Permissions.Environmental.ManageIncidents)]
    public async Task<IActionResult> UpdateIncident(Guid id, UpdateEnvironmentalIncidentCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }
}
