using CoreEngine.API.Filters;
using CoreEngine.Application.Features.Inspections.Commands.CreateFinding;
using CoreEngine.Application.Features.Inspections.Commands.CreateInspection;
using CoreEngine.Application.Features.Inspections.Commands.CreateInspectionTemplate;
using CoreEngine.Application.Features.Inspections.Commands.DeleteInspection;
using CoreEngine.Application.Features.Inspections.Commands.DeleteInspectionTemplate;
using CoreEngine.Application.Features.Inspections.Commands.UpdateFinding;
using CoreEngine.Application.Features.Inspections.Commands.UpdateInspection;
using CoreEngine.Application.Features.Inspections.Commands.UpdateInspectionTemplate;
using CoreEngine.Application.Features.Inspections.DTOs;
using CoreEngine.Application.Features.Inspections.Queries.GetFindings;
using CoreEngine.Application.Features.Inspections.Queries.GetInspectionById;
using CoreEngine.Application.Features.Inspections.Queries.GetInspections;
using CoreEngine.Application.Features.Inspections.Queries.GetInspectionTemplates;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class InspectionsController : BaseApiController
{
    // ── Templates ──────────────────────────────────────────
    [HttpGet("templates")]
    [RequirePermission(Permissions.Inspections.Read)]
    public async Task<ActionResult<IReadOnlyList<InspectionTemplateDto>>> GetTemplates(
        [FromQuery] string? category, [FromQuery] bool? isActive)
        => Ok(await Mediator.Send(new GetInspectionTemplatesQuery(category, isActive)));

    [HttpPost("templates")]
    [RequirePermission(Permissions.Inspections.ManageTemplates)]
    public async Task<ActionResult<Guid>> CreateTemplate(CreateInspectionTemplateCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetTemplates), new { id = result }, result);
    }

    [HttpPut("templates/{id:guid}")]
    [RequirePermission(Permissions.Inspections.ManageTemplates)]
    public async Task<IActionResult> UpdateTemplate(Guid id, UpdateInspectionTemplateCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("templates/{id:guid}")]
    [RequirePermission(Permissions.Inspections.ManageTemplates)]
    public async Task<IActionResult> DeleteTemplate(Guid id)
    {
        await Mediator.Send(new DeleteInspectionTemplateCommand(id));
        return NoContent();
    }

    // ── Inspections ────────────────────────────────────────
    [HttpGet]
    [RequirePermission(Permissions.Inspections.Read)]
    public async Task<ActionResult<IReadOnlyList<InspectionDto>>> GetAll(
        [FromQuery] Guid? mineSiteId, [FromQuery] Guid? templateId, [FromQuery] string? status)
        => Ok(await Mediator.Send(new GetInspectionsQuery(mineSiteId, templateId, status)));

    [HttpGet("{id:guid}")]
    [RequirePermission(Permissions.Inspections.Read)]
    public async Task<ActionResult<InspectionDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetInspectionByIdQuery(id)));

    [HttpPost]
    [RequirePermission(Permissions.Inspections.Create)]
    public async Task<ActionResult<Guid>> Create(CreateInspectionCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(Permissions.Inspections.Update)]
    public async Task<IActionResult> Update(Guid id, UpdateInspectionCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(Permissions.Inspections.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteInspectionCommand(id));
        return NoContent();
    }

    // ── Findings ───────────────────────────────────────────
    [HttpGet("{inspectionId:guid}/findings")]
    [RequirePermission(Permissions.Inspections.Read)]
    public async Task<ActionResult<IReadOnlyList<InspectionFindingDto>>> GetFindings(Guid inspectionId)
        => Ok(await Mediator.Send(new GetFindingsQuery(inspectionId)));

    [HttpPost("{inspectionId:guid}/findings")]
    [RequirePermission(Permissions.Inspections.Create)]
    public async Task<ActionResult<Guid>> CreateFinding(Guid inspectionId, CreateFindingCommand command)
    {
        if (inspectionId != command.InspectionId) return BadRequest("ID mismatch.");
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = inspectionId }, result);
    }

    [HttpPut("findings/{id:guid}")]
    [RequirePermission(Permissions.Inspections.Update)]
    public async Task<IActionResult> UpdateFinding(Guid id, UpdateFindingCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }
}
