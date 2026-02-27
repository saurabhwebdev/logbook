using CoreEngine.API.Filters;
using CoreEngine.Application.Features.Reports.Commands.CreateReport;
using CoreEngine.Application.Features.Reports.Commands.DeleteReport;
using CoreEngine.Application.Features.Reports.Commands.ExportReport;
using CoreEngine.Application.Features.Reports.Queries.GetReports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

[Authorize]
public class ReportsController : BaseApiController
{
    [HttpGet]
    [RequirePermission("Report.Read")]
    public async Task<ActionResult<List<ReportDto>>> GetAll()
        => Ok(await Mediator.Send(new GetReportsQuery()));

    [HttpPost]
    [RequirePermission("Report.Create")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateReportCommand command)
        => Ok(await Mediator.Send(command));

    [HttpDelete("{id:guid}")]
    [RequirePermission("Report.Delete")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteReportCommand(id));
        return NoContent();
    }

    [HttpGet("{id:guid}/export")]
    [RequirePermission("Report.Export")]
    public async Task<IActionResult> Export(Guid id)
    {
        var result = await Mediator.Send(new ExportReportCommand(id));
        return File(result.FileContents, result.ContentType, result.FileName);
    }
}
