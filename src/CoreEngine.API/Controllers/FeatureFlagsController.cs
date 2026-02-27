using CoreEngine.API.Filters;
using CoreEngine.Application.Features.FeatureFlags.Commands.ToggleFeatureFlag;
using CoreEngine.Application.Features.FeatureFlags.Queries.GetFeatureFlags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

[Authorize]
public class FeatureFlagsController : BaseApiController
{
    [HttpGet]
    [RequirePermission("FeatureFlag.Read")]
    public async Task<ActionResult<List<FeatureFlagDto>>> GetAll()
        => Ok(await Mediator.Send(new GetFeatureFlagsQuery()));

    [HttpPut("{id:guid}/toggle")]
    [RequirePermission("FeatureFlag.Update")]
    public async Task<ActionResult> Toggle(Guid id, [FromBody] ToggleFeatureFlagRequest request)
    {
        await Mediator.Send(new ToggleFeatureFlagCommand(id, request.IsEnabled));
        return NoContent();
    }
}

public record ToggleFeatureFlagRequest(bool IsEnabled);
