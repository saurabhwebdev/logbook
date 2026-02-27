using CoreEngine.API.Filters;
using CoreEngine.Application.Features.Help.Commands.CreateHelpArticle;
using CoreEngine.Application.Features.Help.Commands.UpdateHelpArticle;
using CoreEngine.Application.Features.Help.Commands.DeleteHelpArticle;
using CoreEngine.Application.Features.Help.Queries.GetHelpArticles;
using CoreEngine.Application.Features.Help.Queries.GetHelpArticleBySlug;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class HelpController : BaseApiController
{
    [HttpGet]
    [RequirePermission("Help.Read")]
    public async Task<ActionResult<List<HelpArticleDto>>> GetAll(
        [FromQuery] string? category = null,
        [FromQuery] string? moduleKey = null,
        [FromQuery] bool publishedOnly = false)
        => Ok(await Mediator.Send(new GetHelpArticlesQuery(category, moduleKey, publishedOnly)));

    [HttpGet("{slug}")]
    [RequirePermission("Help.Read")]
    public async Task<ActionResult<HelpArticleDto>> GetBySlug(string slug)
    {
        var result = await Mediator.Send(new GetHelpArticleBySlugQuery(slug));
        return result is not null ? Ok(result) : NotFound();
    }

    [HttpPost]
    [RequirePermission("Help.Create")]
    public async Task<ActionResult<Guid>> Create(CreateHelpArticleCommand command)
    {
        var id = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetBySlug), new { slug = command.Slug }, id);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission("Help.Update")]
    public async Task<IActionResult> Update(Guid id, UpdateHelpArticleCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission("Help.Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteHelpArticleCommand(id));
        return NoContent();
    }
}
