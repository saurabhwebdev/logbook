using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Configuration.Commands.UpsertConfiguration;

public record UpsertConfigurationCommand(string Key, string Value, string Category, string? Description, string DataType) : IRequest<Guid>;

public class UpsertConfigurationCommandHandler : IRequestHandler<UpsertConfigurationCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public UpsertConfigurationCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(UpsertConfigurationCommand request, CancellationToken ct)
    {
        var existing = await _context.SystemConfigurations.FirstOrDefaultAsync(c => c.Key == request.Key, ct);
        if (existing is not null)
        {
            existing.Value = request.Value;
            existing.Category = request.Category;
            existing.Description = request.Description;
            existing.DataType = request.DataType;
            await _context.SaveChangesAsync(ct);
            return existing.Id;
        }

        var config = new SystemConfiguration
        {
            Key = request.Key,
            Value = request.Value,
            Category = request.Category,
            Description = request.Description,
            DataType = request.DataType,
        };
        _context.SystemConfigurations.Add(config);
        await _context.SaveChangesAsync(ct);
        return config.Id;
    }
}
