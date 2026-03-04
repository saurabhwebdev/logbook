using CoreEngine.API.Filters;
using CoreEngine.Application.Features.Geotechnical.Commands.CreateGeotechnicalAssessment;
using CoreEngine.Application.Features.Geotechnical.Commands.CreateSurveyRecord;
using CoreEngine.Application.Features.Geotechnical.Commands.DeleteGeotechnicalAssessment;
using CoreEngine.Application.Features.Geotechnical.Commands.DeleteSurveyRecord;
using CoreEngine.Application.Features.Geotechnical.Commands.UpdateGeotechnicalAssessment;
using CoreEngine.Application.Features.Geotechnical.Commands.UpdateSurveyRecord;
using CoreEngine.Application.Features.Geotechnical.DTOs;
using CoreEngine.Application.Features.Geotechnical.Queries.GetGeotechnicalAssessments;
using CoreEngine.Application.Features.Geotechnical.Queries.GetSurveyRecords;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class GeotechnicalController : BaseApiController
{
    // ===== Geotechnical Assessments =====

    [HttpGet("assessments")]
    [RequirePermission(Permissions.Geotechnical.Read)]
    public async Task<ActionResult<IReadOnlyList<GeotechnicalAssessmentDto>>> GetAssessments(
        [FromQuery] Guid? mineSiteId,
        [FromQuery] string? status)
        => Ok(await Mediator.Send(new GetGeotechnicalAssessmentsQuery(mineSiteId, status)));

    [HttpPost("assessments")]
    [RequirePermission(Permissions.Geotechnical.Create)]
    public async Task<ActionResult<Guid>> CreateAssessment(CreateGeotechnicalAssessmentCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetAssessments), new { id = result }, result);
    }

    [HttpPut("assessments/{id:guid}")]
    [RequirePermission(Permissions.Geotechnical.Update)]
    public async Task<IActionResult> UpdateAssessment(Guid id, UpdateGeotechnicalAssessmentCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("assessments/{id:guid}")]
    [RequirePermission(Permissions.Geotechnical.Delete)]
    public async Task<IActionResult> DeleteAssessment(Guid id)
    {
        await Mediator.Send(new DeleteGeotechnicalAssessmentCommand(id));
        return NoContent();
    }

    // ===== Survey Records =====

    [HttpGet("surveys")]
    [RequirePermission(Permissions.Geotechnical.Read)]
    public async Task<ActionResult<IReadOnlyList<SurveyRecordDto>>> GetSurveys(
        [FromQuery] Guid? mineSiteId,
        [FromQuery] string? surveyType)
        => Ok(await Mediator.Send(new GetSurveyRecordsQuery(mineSiteId, surveyType)));

    [HttpPost("surveys")]
    [RequirePermission(Permissions.Geotechnical.Create)]
    public async Task<ActionResult<Guid>> CreateSurvey(CreateSurveyRecordCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetSurveys), new { id = result }, result);
    }

    [HttpPut("surveys/{id:guid}")]
    [RequirePermission(Permissions.Geotechnical.Update)]
    public async Task<IActionResult> UpdateSurvey(Guid id, UpdateSurveyRecordCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("surveys/{id:guid}")]
    [RequirePermission(Permissions.Geotechnical.Delete)]
    public async Task<IActionResult> DeleteSurvey(Guid id)
    {
        await Mediator.Send(new DeleteSurveyRecordCommand(id));
        return NoContent();
    }
}
