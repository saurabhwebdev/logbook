using CoreEngine.API.Filters;
using CoreEngine.Application.Features.Departments.Commands.CreateDepartment;
using CoreEngine.Application.Features.Departments.Commands.DeleteDepartment;
using CoreEngine.Application.Features.Departments.Commands.UpdateDepartment;
using CoreEngine.Application.Features.Departments.Queries.GetDepartments;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class DepartmentsController : BaseApiController
{
    [HttpGet]
    [RequirePermission(Permissions.Departments.Read)]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetAll()
        => Ok(await Mediator.Send(new GetDepartmentsQuery()));

    [HttpPost]
    [RequirePermission(Permissions.Departments.Create)]
    public async Task<ActionResult<Guid>> Create(CreateDepartmentCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), new { id = result }, result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(Permissions.Departments.Update)]
    public async Task<IActionResult> Update(Guid id, UpdateDepartmentCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(Permissions.Departments.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteDepartmentCommand(id));
        return NoContent();
    }
}
