using MediatR;

namespace CoreEngine.Application.Features.Users.Commands.DeleteProfilePhoto;

public record DeleteProfilePhotoCommand : IRequest<Unit>;
