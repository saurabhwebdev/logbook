using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Personnel.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Personnel.Queries.GetPersonnel;

public record GetPersonnelQuery(Guid? MineSiteId, string? Status, string? Role) : IRequest<IReadOnlyList<PersonnelDto>>;

public class GetPersonnelQueryHandler : IRequestHandler<GetPersonnelQuery, IReadOnlyList<PersonnelDto>>
{
    private readonly IApplicationDbContext _context;
    public GetPersonnelQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<IReadOnlyList<PersonnelDto>> Handle(GetPersonnelQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Personnel.AsNoTracking().Include(e => e.MineSite).AsQueryable();

        if (request.MineSiteId.HasValue) query = query.Where(e => e.MineSiteId == request.MineSiteId.Value);
        if (!string.IsNullOrEmpty(request.Status)) query = query.Where(e => e.Status == request.Status);
        if (!string.IsNullOrEmpty(request.Role)) query = query.Where(e => e.Role == request.Role);

        return await query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
            .Select(e => new PersonnelDto(
                e.Id, e.MineSiteId, e.MineSite.Name,
                e.EmployeeNumber, e.FirstName, e.LastName, e.MiddleName,
                e.Role, e.Department, e.Designation,
                e.EmploymentType, e.DateOfJoining, e.DateOfLeaving,
                e.Status, e.ContactPhone, e.ContactEmail,
                e.EmergencyContactName, e.EmergencyContactPhone,
                e.BloodGroup, e.MedicalFitnessCertificate, e.MedicalFitnessExpiry,
                e.Notes, e.Certifications.Count, e.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
