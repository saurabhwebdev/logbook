using FluentValidation;

namespace CoreEngine.Application.Features.MineSites.Commands.CreateMineArea;

public class CreateMineAreaCommandValidator : AbstractValidator<CreateMineAreaCommand>
{
    private static readonly string[] ValidAreaTypes = { "Pit", "Panel", "Level", "Bench", "Stope", "Decline", "Shaft", "ProcessingPlant", "Stockpile", "WasteDump", "TailingsDam", "Workshop", "Magazine", "Office", "Other" };

    public CreateMineAreaCommandValidator()
    {
        RuleFor(x => x.MineSiteId)
            .NotEmpty().WithMessage("Mine site ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Area name is required.")
            .MaximumLength(200).WithMessage("Area name must not exceed 200 characters.");

        RuleFor(x => x.Code)
            .MaximumLength(50).WithMessage("Code must not exceed 50 characters.");

        RuleFor(x => x.AreaType)
            .NotEmpty().WithMessage("Area type is required.")
            .Must(x => ValidAreaTypes.Contains(x)).WithMessage("Invalid area type.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}
