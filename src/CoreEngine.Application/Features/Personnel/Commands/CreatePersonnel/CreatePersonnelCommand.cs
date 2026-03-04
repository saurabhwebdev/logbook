using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Personnel.Commands.CreatePersonnel;

public record CreatePersonnelCommand(
    Guid MineSiteId, string FirstName, string LastName, string? MiddleName,
    string Role, string? Department, string? Designation, string EmploymentType,
    DateTime DateOfJoining, string? ContactPhone, string? ContactEmail,
    string? EmergencyContactName, string? EmergencyContactPhone,
    string? BloodGroup, string? MedicalFitnessCertificate, DateTime? MedicalFitnessExpiry,
    string? Notes) : IRequest<Guid>;

public class CreatePersonnelCommandHandler : IRequestHandler<CreatePersonnelCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreatePersonnelCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreatePersonnelCommand request, CancellationToken cancellationToken)
    {
        var count = await _context.Personnel.CountAsync(cancellationToken);
        var entity = new Domain.Entities.Personnel
        {
            MineSiteId = request.MineSiteId,
            EmployeeNumber = $"EMP-{(count + 1):D5}",
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            Role = request.Role,
            Department = request.Department,
            Designation = request.Designation,
            EmploymentType = request.EmploymentType,
            DateOfJoining = request.DateOfJoining,
            ContactPhone = request.ContactPhone,
            ContactEmail = request.ContactEmail,
            EmergencyContactName = request.EmergencyContactName,
            EmergencyContactPhone = request.EmergencyContactPhone,
            BloodGroup = request.BloodGroup,
            MedicalFitnessCertificate = request.MedicalFitnessCertificate,
            MedicalFitnessExpiry = request.MedicalFitnessExpiry,
            Notes = request.Notes,
        };
        _context.Personnel.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
