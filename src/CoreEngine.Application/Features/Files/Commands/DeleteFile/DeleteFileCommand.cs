using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using MediatR;

namespace CoreEngine.Application.Features.Files.Commands.DeleteFile;

public record DeleteFileCommand(Guid Id) : IRequest;

public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorage;

    public DeleteFileCommandHandler(IApplicationDbContext context, IFileStorageService fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task Handle(DeleteFileCommand request, CancellationToken ct)
    {
        var file = await _context.FileMetadata.FindAsync(new object[] { request.Id }, ct)
            ?? throw new NotFoundException("FileMetadata", request.Id);
        await _fileStorage.DeleteFileAsync(file.StoragePath, ct);
        _context.FileMetadata.Remove(file);
        await _context.SaveChangesAsync(ct);
    }
}
