using CoreEngine.API.Filters;
using CoreEngine.Application.Features.Roles.Commands.CreateRole;
using CoreEngine.Application.Features.Roles.Commands.DeleteRole;
using CoreEngine.Application.Features.Roles.Commands.UpdateRole;
using CoreEngine.Application.Features.Roles.Queries.GetRoles;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class RolesController : BaseApiController
{
    [HttpGet]
    [RequirePermission(Permissions.Roles.Read)]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetAll([FromQuery] GetRolesQuery query)
        => Ok(await Mediator.Send(query));

    [HttpPost]
    [RequirePermission(Permissions.Roles.Create)]
    public async Task<ActionResult<Guid>> Create(CreateRoleCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), new { id = result }, result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(Permissions.Roles.Update)]
    public async Task<IActionResult> Update(Guid id, UpdateRoleCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(Permissions.Roles.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteRoleCommand(id));
        return NoContent();
    }
}
