using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using MediatR;

namespace CoreEngine.Application.Features.ApiIntegration.Commands.RevokeApiKey;

public record RevokeApiKeyCommand(Guid Id) : IRequest;

public class RevokeApiKeyCommandHandler : IRequestHandler<RevokeApiKeyCommand>
{
    private readonly IApplicationDbContext _context;
    public RevokeApiKeyCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(RevokeApiKeyCommand request, CancellationToken ct)
    {
        var apiKey = await _context.ApiKeys.FindAsync(new object[] { request.Id }, ct)
            ?? throw new NotFoundException("ApiKey", request.Id);
        apiKey.IsActive = false;
        await _context.SaveChangesAsync(ct);
    }
}
