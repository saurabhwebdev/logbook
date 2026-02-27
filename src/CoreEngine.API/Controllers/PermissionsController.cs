using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

[Route("api/[controller]")]
public class PermissionsController : BaseApiController
{
    [HttpGet]
    [Authorize]
    public ActionResult<IReadOnlyList<string>> GetAll()
        => Ok(Permissions.GetAll());

    [HttpGet("grouped")]
    [Authorize]
    public ActionResult<Dictionary<string, List<string>>> GetGrouped()
    {
        var permissions = Permissions.GetAll();

        var grouped = permissions
            .GroupBy(p => p.Split('.')[0])
            .ToDictionary(g => g.Key, g => g.ToList());

        return Ok(grouped);
    }
}
