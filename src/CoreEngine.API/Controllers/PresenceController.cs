using CoreEngine.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

[Authorize]
public class PresenceController : BaseApiController
{
    private readonly IUserPresenceService _presenceService;

    public PresenceController(IUserPresenceService presenceService)
    {
        _presenceService = presenceService;
    }

    [HttpGet("online-users")]
    public async Task<ActionResult<List<string>>> GetOnlineUsers()
    {
        var onlineUsers = await _presenceService.GetOnlineUsersAsync();
        return Ok(onlineUsers);
    }

    [HttpGet("is-online/{userId}")]
    public async Task<ActionResult<bool>> IsUserOnline(string userId)
    {
        var isOnline = await _presenceService.IsUserOnlineAsync(userId);
        return Ok(isOnline);
    }
}
