using CoreEngine.Application.Features.Search.Queries.GlobalSearch;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class SearchController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<GlobalSearchResultDto>> GlobalSearch([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest("Search term is required.");
        }

        var result = await Mediator.Send(new GlobalSearchQuery(q));
        return Ok(result);
    }
}
