using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;

namespace CoreEngine.Application.Features.ApiIntegration.Commands.CreateApiKey;

public record CreateApiKeyResponse(Guid Id, string RawKey);

public record CreateApiKeyCommand(string Name, string? Scopes, DateTime? ExpiresAt) : IRequest<CreateApiKeyResponse>;

public class CreateApiKeyCommandHandler : IRequestHandler<CreateApiKeyCommand, CreateApiKeyResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public CreateApiKeyCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<CreateApiKeyResponse> Handle(CreateApiKeyCommand request, CancellationToken ct)
    {
        var rawKey = $"ce_{Guid.NewGuid():N}";
        var keyHash = _passwordHasher.Hash(rawKey);

        var apiKey = new ApiKey
        {
            Name = request.Name,
            KeyHash = keyHash,
            KeyPrefix = rawKey[..12],
            Scopes = request.Scopes,
            ExpiresAt = request.ExpiresAt,
        };
        _context.ApiKeys.Add(apiKey);
        await _context.SaveChangesAsync(ct);
        return new CreateApiKeyResponse(apiKey.Id, rawKey);
    }
}
