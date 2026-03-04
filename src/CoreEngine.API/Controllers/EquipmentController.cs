using CoreEngine.API.Filters;
using CoreEngine.Application.Features.Equipment.Commands.CreateEquipment;
using CoreEngine.Application.Features.Equipment.Commands.CreateMaintenanceRecord;
using CoreEngine.Application.Features.Equipment.Commands.DeleteEquipment;
using CoreEngine.Application.Features.Equipment.Commands.UpdateEquipment;
using CoreEngine.Application.Features.Equipment.Commands.UpdateMaintenanceRecord;
using CoreEngine.Application.Features.Equipment.DTOs;
using CoreEngine.Application.Features.Equipment.Queries.GetEquipment;
using CoreEngine.Application.Features.Equipment.Queries.GetEquipmentById;
using CoreEngine.Application.Features.Equipment.Queries.GetMaintenanceRecords;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class EquipmentController : BaseApiController
{
    [HttpGet]
    [RequirePermission(Permissions.EquipmentMgmt.Read)]
    public async Task<ActionResult<IReadOnlyList<EquipmentDto>>> GetAll(
        [FromQuery] Guid? mineSiteId, [FromQuery] string? category, [FromQuery] string? status)
        => Ok(await Mediator.Send(new GetEquipmentQuery(mineSiteId, category, status)));

    [HttpGet("{id:guid}")]
    [RequirePermission(Permissions.EquipmentMgmt.Read)]
    public async Task<ActionResult<EquipmentDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetEquipmentByIdQuery(id)));

    [HttpPost]
    [RequirePermission(Permissions.EquipmentMgmt.Create)]
    public async Task<ActionResult<Guid>> Create(CreateEquipmentCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(Permissions.EquipmentMgmt.Update)]
    public async Task<IActionResult> Update(Guid id, UpdateEquipmentCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(Permissions.EquipmentMgmt.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteEquipmentCommand(id));
        return NoContent();
    }

    // Maintenance Records
    [HttpGet("maintenance")]
    [RequirePermission(Permissions.EquipmentMgmt.Read)]
    public async Task<ActionResult<IReadOnlyList<MaintenanceRecordDto>>> GetMaintenanceRecords(
        [FromQuery] Guid? equipmentId, [FromQuery] string? status)
        => Ok(await Mediator.Send(new GetMaintenanceRecordsQuery(equipmentId, status)));

    [HttpPost("{equipmentId:guid}/maintenance")]
    [RequirePermission(Permissions.EquipmentMgmt.Maintain)]
    public async Task<ActionResult<Guid>> CreateMaintenance(Guid equipmentId, CreateMaintenanceRecordCommand command)
    {
        if (equipmentId != command.EquipmentId) return BadRequest("ID mismatch.");
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = equipmentId }, result);
    }

    [HttpPut("maintenance/{id:guid}")]
    [RequirePermission(Permissions.EquipmentMgmt.Maintain)]
    public async Task<IActionResult> UpdateMaintenance(Guid id, UpdateMaintenanceRecordCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }
}
