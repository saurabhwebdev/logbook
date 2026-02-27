using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Users.Commands.UploadProfilePhoto;

public class UploadProfilePhotoCommandHandler : IRequestHandler<UploadProfilePhotoCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly ICurrentUserService _currentUserService;

    public UploadProfilePhotoCommandHandler(
        IApplicationDbContext context,
        IFileStorageService fileStorage,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _fileStorage = fileStorage;
        _currentUserService = currentUserService;
    }

    public async Task<string> Handle(UploadProfilePhotoCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Validate file type (only images)
        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(request.ContentType.ToLower()))
        {
            throw new InvalidOperationException("Only image files are allowed for profile photos");
        }

        // Validate file size (max 5MB)
        if (request.FileSize > 5_000_000)
        {
            throw new InvalidOperationException("Profile photo must be less than 5MB");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId), cancellationToken)
            ?? throw new KeyNotFoundException("User not found");

        // Delete old profile photo if exists
        if (!string.IsNullOrEmpty(user.ProfilePhotoUrl))
        {
            try
            {
                await _fileStorage.DeleteFileAsync(user.ProfilePhotoUrl, cancellationToken);
            }
            catch
            {
                // Ignore errors when deleting old photo
            }
        }

        // Save new photo with user ID prefix for easy identification
        var fileName = $"profile_{userId}_{Path.GetFileName(request.FileName)}";
        var storagePath = await _fileStorage.SaveFileAsync(request.FileStream, fileName, cancellationToken);

        // Update user profile photo URL
        user.ProfilePhotoUrl = storagePath;
        await _context.SaveChangesAsync(cancellationToken);

        return storagePath;
    }
}
