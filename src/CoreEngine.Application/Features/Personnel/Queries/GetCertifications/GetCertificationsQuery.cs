using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Personnel.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Personnel.Queries.GetCertifications;

public record GetCertificationsQuery(Guid PersonnelId) : IRequest<IReadOnlyList<PersonnelCertificationDto>>;

public class GetCertificationsQueryHandler : IRequestHandler<GetCertificationsQuery, IReadOnlyList<PersonnelCertificationDto>>
{
    private readonly IApplicationDbContext _context;
    public GetCertificationsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<IReadOnlyList<PersonnelCertificationDto>> Handle(GetCertificationsQuery request, CancellationToken cancellationToken)
    {
        return await _context.PersonnelCertifications.AsNoTracking()
            .Include(e => e.Personnel)
            .Where(e => e.PersonnelId == request.PersonnelId)
            .OrderByDescending(e => e.ExpiryDate)
            .Select(e => new PersonnelCertificationDto(
                e.Id, e.PersonnelId, e.Personnel.FirstName + " " + e.Personnel.LastName,
                e.CertificationName, e.CertificateNumber, e.IssuingAuthority,
                e.IssueDate, e.ExpiryDate, e.Status, e.Category,
                e.Notes, e.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
