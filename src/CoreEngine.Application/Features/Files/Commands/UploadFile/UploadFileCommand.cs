using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;

namespace CoreEngine.Application.Features.Files.Commands.UploadFile;

public record UploadFileCommand(Stream FileStream, string OriginalFileName, string ContentType, long FileSize, string? Description, string? Category) : IRequest<Guid>;

public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, Guid>
{
    private readonly IFileStorageService _fileStorage;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UploadFileCommandHandler(IFileStorageService fileStorage, IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _fileStorage = fileStorage;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(UploadFileCommand request, CancellationToken ct)
    {
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.OriginalFileName)}";
        var storagePath = await _fileStorage.SaveFileAsync(request.FileStream, fileName, ct);

        var file = new FileMetadata
        {
            FileName = fileName,
            OriginalFileName = request.OriginalFileName,
            ContentType = request.ContentType,
            FileSize = request.FileSize,
            StoragePath = storagePath,
            Description = request.Description,
            Category = request.Category,
            UploadedByUserId = Guid.TryParse(_currentUser.UserId, out var uid) ? uid : Guid.Empty,
        };
        _context.FileMetadata.Add(file);
        await _context.SaveChangesAsync(ct);
        return file.Id;
    }
}
