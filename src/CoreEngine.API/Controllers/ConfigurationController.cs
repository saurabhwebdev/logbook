using CoreEngine.API.Filters;
using CoreEngine.Application.Features.Configuration.Commands.UpsertConfiguration;
using CoreEngine.Application.Features.Configuration.Queries.GetConfigurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

[Authorize]
public class ConfigurationController : BaseApiController
{
    [HttpGet]
    [RequirePermission("Configuration.Read")]
    public async Task<ActionResult<List<ConfigurationDto>>> GetAll([FromQuery] string? category)
        => Ok(await Mediator.Send(new GetConfigurationsQuery(category)));

    [HttpPost]
    [RequirePermission("Configuration.Update")]
    public async Task<ActionResult<Guid>> Upsert([FromBody] UpsertConfigurationCommand command)
        => Ok(await Mediator.Send(command));
}
