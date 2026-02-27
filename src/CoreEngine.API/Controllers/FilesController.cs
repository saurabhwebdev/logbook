using CoreEngine.API.Filters;
using CoreEngine.Application.Features.Files.Commands.DeleteFile;
using CoreEngine.Application.Features.Files.Commands.UploadFile;
using CoreEngine.Application.Features.Files.Queries.GetFiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

[Authorize]
public class FilesController : BaseApiController
{
    [HttpGet]
    [RequirePermission("File.Read")]
    public async Task<ActionResult<List<FileDto>>> GetAll([FromQuery] string? category)
        => Ok(await Mediator.Send(new GetFilesQuery(category)));

    [HttpPost("upload")]
    [RequirePermission("File.Upload")]
    [RequestSizeLimit(50_000_000)] // 50MB
    public async Task<ActionResult<Guid>> Upload(IFormFile file, [FromForm] string? description, [FromForm] string? category)
    {
        using var stream = file.OpenReadStream();
        var command = new UploadFileCommand(stream, file.FileName, file.ContentType, file.Length, description, category);
        var id = await Mediator.Send(command);
        return Ok(id);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission("File.Delete")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteFileCommand(id));
        return NoContent();
    }
}
