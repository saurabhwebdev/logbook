using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Files.Queries.GetFiles;

public record FileDto(Guid Id, string FileName, string OriginalFileName, string ContentType, long FileSize, string? Description, string? Category, string? UploadedByName, DateTime CreatedAt);

public record GetFilesQuery(string? Category = null) : IRequest<List<FileDto>>;

public class GetFilesQueryHandler : IRequestHandler<GetFilesQuery, List<FileDto>>
{
    private readonly IApplicationDbContext _context;
    public GetFilesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<FileDto>> Handle(GetFilesQuery request, CancellationToken ct)
    {
        var query = _context.FileMetadata.AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Category))
            query = query.Where(f => f.Category == request.Category);
        return await query.OrderByDescending(f => f.CreatedAt)
            .Select(f => new FileDto(f.Id, f.FileName, f.OriginalFileName, f.ContentType, f.FileSize, f.Description, f.Category, f.UploadedByName, f.CreatedAt))
            .ToListAsync(ct);
    }
}
