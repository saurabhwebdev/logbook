using MediatR;

namespace CoreEngine.Application.Features.Users.Commands.UploadProfilePhoto;

public record UploadProfilePhotoCommand(
    Stream FileStream,
    string FileName,
    string ContentType,
    long FileSize
) : IRequest<string>;
