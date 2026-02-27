using CoreEngine.API.Filters;
using CoreEngine.Application.Features.FeatureFlags.Commands.CreateFeatureFlag;
using CoreEngine.Application.Features.FeatureFlags.Commands.DeleteFeatureFlag;
using CoreEngine.Application.Features.FeatureFlags.Commands.ToggleFeatureFlag;
using CoreEngine.Application.Features.FeatureFlags.Commands.UpdateFeatureFlag;
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

    [HttpPost]
    [RequirePermission("FeatureFlag.Create")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateFeatureFlagRequest request)
    {
        var id = await Mediator.Send(new CreateFeatureFlagCommand(
            request.Name,
            request.Description,
            request.IsEnabled
        ));
        return Ok(id);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission("FeatureFlag.Update")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateFeatureFlagRequest request)
    {
        await Mediator.Send(new UpdateFeatureFlagCommand(id, request.Name, request.Description));
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission("FeatureFlag.Delete")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteFeatureFlagCommand(id));
        return NoContent();
    }

    [HttpPut("{id:guid}/toggle")]
    [RequirePermission("FeatureFlag.Update")]
    public async Task<ActionResult> Toggle(Guid id, [FromBody] ToggleFeatureFlagRequest request)
    {
        await Mediator.Send(new ToggleFeatureFlagCommand(id, request.IsEnabled));
        return NoContent();
    }
}

public record CreateFeatureFlagRequest(string Name, string? Description, bool IsEnabled = false);
public record UpdateFeatureFlagRequest(string Name, string? Description);
public record ToggleFeatureFlagRequest(bool IsEnabled);
