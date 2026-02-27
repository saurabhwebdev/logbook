using CoreEngine.API.Filters;
using CoreEngine.Application.Features.ApiIntegration.Commands.CreateApiKey;
using CoreEngine.Application.Features.ApiIntegration.Commands.CreateWebhook;
using CoreEngine.Application.Features.ApiIntegration.Commands.DeleteWebhook;
using CoreEngine.Application.Features.ApiIntegration.Commands.RevokeApiKey;
using CoreEngine.Application.Features.ApiIntegration.Queries.GetApiKeys;
using CoreEngine.Application.Features.ApiIntegration.Queries.GetWebhooks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

[Authorize]
public class ApiIntegrationController : BaseApiController
{
    // API Keys
    [HttpGet("keys")]
    [RequirePermission("ApiIntegration.Read")]
    public async Task<ActionResult<List<ApiKeyDto>>> GetApiKeys()
        => Ok(await Mediator.Send(new GetApiKeysQuery()));

    [HttpPost("keys")]
    [RequirePermission("ApiIntegration.Manage")]
    public async Task<ActionResult<CreateApiKeyResponse>> CreateApiKey([FromBody] CreateApiKeyCommand command)
        => Ok(await Mediator.Send(command));

    [HttpPut("keys/{id:guid}/revoke")]
    [RequirePermission("ApiIntegration.Manage")]
    public async Task<ActionResult> RevokeApiKey(Guid id)
    {
        await Mediator.Send(new RevokeApiKeyCommand(id));
        return NoContent();
    }

    // Webhooks
    [HttpGet("webhooks")]
    [RequirePermission("ApiIntegration.Read")]
    public async Task<ActionResult<List<WebhookDto>>> GetWebhooks()
        => Ok(await Mediator.Send(new GetWebhooksQuery()));

    [HttpPost("webhooks")]
    [RequirePermission("ApiIntegration.Manage")]
    public async Task<ActionResult<CreateWebhookResponse>> CreateWebhook([FromBody] CreateWebhookCommand command)
        => Ok(await Mediator.Send(command));

    [HttpDelete("webhooks/{id:guid}")]
    [RequirePermission("ApiIntegration.Manage")]
    public async Task<ActionResult> DeleteWebhook(Guid id)
    {
        await Mediator.Send(new DeleteWebhookCommand(id));
        return NoContent();
    }
}
