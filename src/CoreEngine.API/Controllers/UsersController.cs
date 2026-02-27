using CoreEngine.API.Filters;
using CoreEngine.Application.Common.Models;
using CoreEngine.Application.Features.Users.Commands.CreateUser;
using CoreEngine.Application.Features.Users.Commands.DeleteUser;
using CoreEngine.Application.Features.Users.Commands.UpdateUser;
using CoreEngine.Application.Features.Users.Commands.UploadProfilePhoto;
using CoreEngine.Application.Features.Users.Commands.DeleteProfilePhoto;
using CoreEngine.Application.Features.Users.Queries.GetUserById;
using CoreEngine.Application.Features.Users.Queries.GetUsers;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class UsersController : BaseApiController
{
    [HttpGet]
    [RequirePermission(Permissions.Users.Read)]
    public async Task<ActionResult<PaginatedList<UserDto>>> GetAll([FromQuery] GetUsersQuery query)
        => Ok(await Mediator.Send(query));

    [HttpGet("{id:guid}")]
    [RequirePermission(Permissions.Users.Read)]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetUserByIdQuery(id)));

    [HttpPost]
    [RequirePermission(Permissions.Users.Create)]
    public async Task<ActionResult<Guid>> Create(CreateUserCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(Permissions.Users.Update)]
    public async Task<IActionResult> Update(Guid id, UpdateUserCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(Permissions.Users.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }

    [HttpPost("profile-photo")]
    [RequestSizeLimit(5_000_000)] // 5MB
    public async Task<ActionResult<string>> UploadProfilePhoto(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var command = new UploadProfilePhotoCommand(stream, file.FileName, file.ContentType, file.Length);
        var url = await Mediator.Send(command);
        return Ok(url);
    }

    [HttpDelete("profile-photo")]
    public async Task<IActionResult> DeleteProfilePhoto()
    {
        await Mediator.Send(new DeleteProfilePhotoCommand());
        return NoContent();
    }
}
