using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Personnel.Commands.UpdatePersonnel;

public record UpdatePersonnelCommand(
    Guid Id, string FirstName, string LastName, string? MiddleName,
    string Role, string? Department, string? Designation, string EmploymentType,
    DateTime DateOfJoining, DateTime? DateOfLeaving, string Status,
    string? ContactPhone, string? ContactEmail,
    string? EmergencyContactName, string? EmergencyContactPhone,
    string? BloodGroup, string? MedicalFitnessCertificate, DateTime? MedicalFitnessExpiry,
    string? Notes) : IRequest;

public class UpdatePersonnelCommandHandler : IRequestHandler<UpdatePersonnelCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdatePersonnelCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdatePersonnelCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Personnel
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Personnel {request.Id} not found.");

        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.MiddleName = request.MiddleName;
        entity.Role = request.Role;
        entity.Department = request.Department;
        entity.Designation = request.Designation;
        entity.EmploymentType = request.EmploymentType;
        entity.DateOfJoining = request.DateOfJoining;
        entity.DateOfLeaving = request.DateOfLeaving;
        entity.Status = request.Status;
        entity.ContactPhone = request.ContactPhone;
        entity.ContactEmail = request.ContactEmail;
        entity.EmergencyContactName = request.EmergencyContactName;
        entity.EmergencyContactPhone = request.EmergencyContactPhone;
        entity.BloodGroup = request.BloodGroup;
        entity.MedicalFitnessCertificate = request.MedicalFitnessCertificate;
        entity.MedicalFitnessExpiry = request.MedicalFitnessExpiry;
        entity.Notes = request.Notes;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
