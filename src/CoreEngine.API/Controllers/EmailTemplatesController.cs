using CoreEngine.API.Filters;
using CoreEngine.Application.Features.EmailTemplates.Commands.CreateEmailTemplate;
using CoreEngine.Application.Features.EmailTemplates.Commands.DeleteEmailTemplate;
using CoreEngine.Application.Features.EmailTemplates.Commands.UpdateEmailTemplate;
using CoreEngine.Application.Features.EmailTemplates.Queries.GetEmailTemplateById;
using CoreEngine.Application.Features.EmailTemplates.Queries.GetEmailTemplates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

[Authorize]
public class EmailTemplatesController : BaseApiController
{
    [HttpGet]
    [RequirePermission("EmailTemplate.Read")]
    public async Task<ActionResult<List<EmailTemplateDto>>> GetAll()
        => Ok(await Mediator.Send(new GetEmailTemplatesQuery()));

    [HttpGet("{id:guid}")]
    [RequirePermission("EmailTemplate.Read")]
    public async Task<ActionResult<EmailTemplateDto>> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetEmailTemplateByIdQuery(id));
        return result != null ? Ok(result) : NotFound();
    }

    [HttpPost]
    [RequirePermission("EmailTemplate.Create")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateEmailTemplateCommand command)
        => Ok(await Mediator.Send(command));

    [HttpPut("{id:guid}")]
    [RequirePermission("EmailTemplate.Update")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateEmailTemplateCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission("EmailTemplate.Delete")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteEmailTemplateCommand(id));
        return NoContent();
    }
}
