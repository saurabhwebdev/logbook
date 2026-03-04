using CoreEngine.API.Filters;
using CoreEngine.Application.Features.Compliance.Commands.CreateComplianceAudit;
using CoreEngine.Application.Features.Compliance.Commands.CreateComplianceRequirement;
using CoreEngine.Application.Features.Compliance.Commands.DeleteComplianceRequirement;
using CoreEngine.Application.Features.Compliance.Commands.UpdateComplianceRequirement;
using CoreEngine.Application.Features.Compliance.DTOs;
using CoreEngine.Application.Features.Compliance.Queries.GetComplianceAudits;
using CoreEngine.Application.Features.Compliance.Queries.GetComplianceRequirements;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class ComplianceController : BaseApiController
{
    // ===== Compliance Requirements =====

    [HttpGet("requirements")]
    [RequirePermission(Permissions.Compliance.Read)]
    public async Task<ActionResult<IReadOnlyList<ComplianceRequirementDto>>> GetRequirements(
        [FromQuery] Guid? mineSiteId,
        [FromQuery] string? status,
        [FromQuery] string? category)
        => Ok(await Mediator.Send(new GetComplianceRequirementsQuery(mineSiteId, status, category)));

    [HttpPost("requirements")]
    [RequirePermission(Permissions.Compliance.Create)]
    public async Task<ActionResult<Guid>> CreateRequirement(CreateComplianceRequirementCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetRequirements), new { id = result }, result);
    }

    [HttpPut("requirements/{id:guid}")]
    [RequirePermission(Permissions.Compliance.Update)]
    public async Task<IActionResult> UpdateRequirement(Guid id, UpdateComplianceRequirementCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("requirements/{id:guid}")]
    [RequirePermission(Permissions.Compliance.Delete)]
    public async Task<IActionResult> DeleteRequirement(Guid id)
    {
        await Mediator.Send(new DeleteComplianceRequirementCommand(id));
        return NoContent();
    }

    // ===== Compliance Audits =====

    [HttpGet("requirements/{requirementId:guid}/audits")]
    [RequirePermission(Permissions.Compliance.Read)]
    public async Task<ActionResult<IReadOnlyList<ComplianceAuditDto>>> GetAudits(Guid requirementId)
        => Ok(await Mediator.Send(new GetComplianceAuditsQuery(requirementId)));

    [HttpPost("audits")]
    [RequirePermission(Permissions.Compliance.Audit)]
    public async Task<ActionResult<Guid>> CreateAudit(CreateComplianceAuditCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetAudits), new { requirementId = command.ComplianceRequirementId, id = result }, result);
    }
}
