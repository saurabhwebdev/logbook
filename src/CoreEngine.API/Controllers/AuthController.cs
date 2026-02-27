using CoreEngine.Application.Features.Auth.Commands.Login;
using CoreEngine.Application.Features.Auth.Commands.Logout;
using CoreEngine.Application.Features.Auth.Commands.RefreshToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

[Route("api/[controller]")]
public class AuthController : BaseApiController
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginCommand command)
        => Ok(await Mediator.Send(command));

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh(RefreshTokenCommand command)
        => Ok(await Mediator.Send(command));

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }
}
