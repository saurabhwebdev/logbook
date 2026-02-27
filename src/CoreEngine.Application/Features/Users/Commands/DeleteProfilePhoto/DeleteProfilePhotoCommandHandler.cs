using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Users.Commands.DeleteProfilePhoto;

public class DeleteProfilePhotoCommandHandler : IRequestHandler<DeleteProfilePhotoCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly ICurrentUserService _currentUserService;

    public DeleteProfilePhotoCommandHandler(
        IApplicationDbContext context,
        IFileStorageService fileStorage,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _fileStorage = fileStorage;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeleteProfilePhotoCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId), cancellationToken)
            ?? throw new KeyNotFoundException("User not found");

        if (string.IsNullOrEmpty(user.ProfilePhotoUrl))
        {
            return Unit.Value; // Nothing to delete
        }

        // Delete file from storage
        await _fileStorage.DeleteFileAsync(user.ProfilePhotoUrl, cancellationToken);

        // Clear profile photo URL
        user.ProfilePhotoUrl = null;
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
