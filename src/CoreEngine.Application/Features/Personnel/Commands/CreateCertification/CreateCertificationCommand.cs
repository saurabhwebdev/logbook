using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;

namespace CoreEngine.Application.Features.Personnel.Commands.CreateCertification;

public record CreateCertificationCommand(
    Guid PersonnelId, string CertificationName, string? CertificateNumber,
    string? IssuingAuthority, DateTime IssueDate, DateTime? ExpiryDate,
    string? Category, string? Notes) : IRequest<Guid>;

public class CreateCertificationCommandHandler : IRequestHandler<CreateCertificationCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateCertificationCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateCertificationCommand request, CancellationToken cancellationToken)
    {
        var entity = new PersonnelCertification
        {
            PersonnelId = request.PersonnelId,
            CertificationName = request.CertificationName,
            CertificateNumber = request.CertificateNumber,
            IssuingAuthority = request.IssuingAuthority,
            IssueDate = request.IssueDate,
            ExpiryDate = request.ExpiryDate,
            Category = request.Category,
            Notes = request.Notes,
        };
        _context.PersonnelCertifications.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
