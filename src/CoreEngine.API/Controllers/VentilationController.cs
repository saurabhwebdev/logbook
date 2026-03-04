using CoreEngine.API.Filters;
using CoreEngine.Application.Features.Ventilation.Commands.CreateGasReading;
using CoreEngine.Application.Features.Ventilation.Commands.CreateVentilationReading;
using CoreEngine.Application.Features.Ventilation.Commands.UpdateGasReading;
using CoreEngine.Application.Features.Ventilation.Commands.UpdateVentilationReading;
using CoreEngine.Application.Features.Ventilation.DTOs;
using CoreEngine.Application.Features.Ventilation.Queries.GetGasReadings;
using CoreEngine.Application.Features.Ventilation.Queries.GetVentilationReadings;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class VentilationController : BaseApiController
{
    // ===== Ventilation Readings =====

    [HttpGet("readings")]
    [RequirePermission(Permissions.Ventilation.Read)]
    public async Task<ActionResult<IReadOnlyList<VentilationReadingDto>>> GetVentilationReadings(
        [FromQuery] Guid? mineSiteId,
        [FromQuery] string? status)
        => Ok(await Mediator.Send(new GetVentilationReadingsQuery(mineSiteId, status)));

    [HttpPost("readings")]
    [RequirePermission(Permissions.Ventilation.Create)]
    public async Task<ActionResult<Guid>> CreateVentilationReading(CreateVentilationReadingCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetVentilationReadings), new { id = result }, result);
    }

    [HttpPut("readings/{id:guid}")]
    [RequirePermission(Permissions.Ventilation.Update)]
    public async Task<IActionResult> UpdateVentilationReading(Guid id, UpdateVentilationReadingCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    // ===== Gas Readings =====

    [HttpGet("gas-readings")]
    [RequirePermission(Permissions.Ventilation.Read)]
    public async Task<ActionResult<IReadOnlyList<GasReadingDto>>> GetGasReadings(
        [FromQuery] Guid? mineSiteId,
        [FromQuery] string? gasType,
        [FromQuery] string? status)
        => Ok(await Mediator.Send(new GetGasReadingsQuery(mineSiteId, gasType, status)));

    [HttpPost("gas-readings")]
    [RequirePermission(Permissions.Ventilation.Create)]
    public async Task<ActionResult<Guid>> CreateGasReading(CreateGasReadingCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetGasReadings), new { id = result }, result);
    }

    [HttpPut("gas-readings/{id:guid}")]
    [RequirePermission(Permissions.Ventilation.Update)]
    public async Task<IActionResult> UpdateGasReading(Guid id, UpdateGasReadingCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }
}
