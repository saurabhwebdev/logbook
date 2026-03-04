using CoreEngine.API.Filters;
using CoreEngine.Application.Features.Production.Commands.CreateDispatchRecord;
using CoreEngine.Application.Features.Production.Commands.CreateProductionLog;
using CoreEngine.Application.Features.Production.Commands.DeleteDispatchRecord;
using CoreEngine.Application.Features.Production.Commands.DeleteProductionLog;
using CoreEngine.Application.Features.Production.Commands.UpdateDispatchRecord;
using CoreEngine.Application.Features.Production.Commands.UpdateProductionLog;
using CoreEngine.Application.Features.Production.DTOs;
using CoreEngine.Application.Features.Production.Queries.GetDispatchRecords;
using CoreEngine.Application.Features.Production.Queries.GetProductionLogs;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class ProductionController : BaseApiController
{
    // ===== Production Logs =====

    [HttpGet("logs")]
    [RequirePermission(Permissions.Production.Read)]
    public async Task<ActionResult<IReadOnlyList<ProductionLogDto>>> GetProductionLogs(
        [FromQuery] Guid? mineSiteId,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] string? material)
        => Ok(await Mediator.Send(new GetProductionLogsQuery(mineSiteId, dateFrom, dateTo, material)));

    [HttpPost("logs")]
    [RequirePermission(Permissions.Production.Create)]
    public async Task<ActionResult<Guid>> CreateProductionLog(CreateProductionLogCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetProductionLogs), new { id = result }, result);
    }

    [HttpPut("logs/{id:guid}")]
    [RequirePermission(Permissions.Production.Update)]
    public async Task<IActionResult> UpdateProductionLog(Guid id, UpdateProductionLogCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("logs/{id:guid}")]
    [RequirePermission(Permissions.Production.Delete)]
    public async Task<IActionResult> DeleteProductionLog(Guid id)
    {
        await Mediator.Send(new DeleteProductionLogCommand(id));
        return NoContent();
    }

    // ===== Dispatch Records =====

    [HttpGet("dispatch")]
    [RequirePermission(Permissions.Dispatch.Read)]
    public async Task<ActionResult<IReadOnlyList<DispatchRecordDto>>> GetDispatchRecords(
        [FromQuery] Guid? mineSiteId,
        [FromQuery] string? status)
        => Ok(await Mediator.Send(new GetDispatchRecordsQuery(mineSiteId, status)));

    [HttpPost("dispatch")]
    [RequirePermission(Permissions.Dispatch.Create)]
    public async Task<ActionResult<Guid>> CreateDispatchRecord(CreateDispatchRecordCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetDispatchRecords), new { id = result }, result);
    }

    [HttpPut("dispatch/{id:guid}")]
    [RequirePermission(Permissions.Dispatch.Update)]
    public async Task<IActionResult> UpdateDispatchRecord(Guid id, UpdateDispatchRecordCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("dispatch/{id:guid}")]
    [RequirePermission(Permissions.Dispatch.Delete)]
    public async Task<IActionResult> DeleteDispatchRecord(Guid id)
    {
        await Mediator.Send(new DeleteDispatchRecordCommand(id));
        return NoContent();
    }
}
