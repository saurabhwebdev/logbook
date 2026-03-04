using FluentValidation;

namespace CoreEngine.Application.Features.MineSites.Commands.CreateMineSite;

public class CreateMineSiteCommandValidator : AbstractValidator<CreateMineSiteCommand>
{
    private static readonly string[] ValidMineTypes = { "Underground", "OpenPit", "Mixed" };
    private static readonly string[] ValidJurisdictions = { "MSHA", "DGMS", "AU_QLD", "AU_NSW", "AU_WA", "AU_SA", "AU_TAS", "AU_VIC", "SA_MHSA", "CANADA_BC", "CANADA_ON", "CANADA_QC", "CHILE_SERNAGEOMIN", "PERU_OSINERGMIN", "OTHER" };
    private static readonly string[] ValidStatuses = { "Active", "Suspended", "Closed", "UnderConstruction" };
    private static readonly string[] ValidUnitSystems = { "Metric", "Imperial" };

    public CreateMineSiteCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Mine site name is required.")
            .MaximumLength(300).WithMessage("Mine site name must not exceed 300 characters.");

        RuleFor(x => x.Code)
            .MaximumLength(50).WithMessage("Code must not exceed 50 characters.");

        RuleFor(x => x.MineType)
            .NotEmpty().WithMessage("Mine type is required.")
            .Must(x => ValidMineTypes.Contains(x)).WithMessage("Mine type must be Underground, OpenPit, or Mixed.");

        RuleFor(x => x.Jurisdiction)
            .NotEmpty().WithMessage("Jurisdiction is required.")
            .Must(x => ValidJurisdictions.Contains(x)).WithMessage("Invalid jurisdiction.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue)
            .WithMessage("Latitude must be between -90 and 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue)
            .WithMessage("Longitude must be between -180 and 180.");

        RuleFor(x => x.Status)
            .Must(x => x == null || ValidStatuses.Contains(x)).WithMessage("Invalid status.");

        RuleFor(x => x.UnitSystem)
            .Must(x => x == null || ValidUnitSystems.Contains(x)).WithMessage("Unit system must be Metric or Imperial.");

        RuleFor(x => x.ShiftsPerDay)
            .InclusiveBetween(1, 4).When(x => x.ShiftsPerDay.HasValue)
            .WithMessage("Shifts per day must be between 1 and 4.");
    }
}
