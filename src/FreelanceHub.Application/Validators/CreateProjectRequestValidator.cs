using FluentValidation;
using FreelanceHub.Application.DTOs.Projects;

namespace FreelanceHub.Application.Validators;

public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
{
    private static readonly string[] ValidTypes = { "Fixed", "Hourly" };
    private static readonly string[] ValidExperienceLevels = { "Entry", "Intermediate", "Expert" };
    private static readonly string[] ValidDurationUnits = { "Hours", "Days", "Weeks", "Months" };

    public CreateProjectRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(10).WithMessage("Title must be at least 10 characters")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(50).WithMessage("Description must be at least 50 characters")
            .MaximumLength(5000).WithMessage("Description cannot exceed 5000 characters");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .MaximumLength(100).WithMessage("Category cannot exceed 100 characters");

        RuleFor(x => x.Budget)
            .GreaterThan(0).WithMessage("Budget must be greater than 0")
            .LessThanOrEqualTo(1_000_000).WithMessage("Budget cannot exceed 1,000,000");

        RuleFor(x => x.Type)
            .Must(t => ValidTypes.Contains(t))
            .WithMessage($"Type must be one of: {string.Join(", ", ValidTypes)}");

        RuleFor(x => x.RequiredExperienceLevel)
            .Must(l => ValidExperienceLevels.Contains(l))
            .WithMessage($"Experience level must be one of: {string.Join(", ", ValidExperienceLevels)}");

        RuleFor(x => x.EstimatedDuration)
            .GreaterThan(0).WithMessage("Estimated duration must be greater than 0")
            .When(x => x.EstimatedDuration > 0);

        RuleFor(x => x.DurationUnit)
            .Must(u => ValidDurationUnits.Contains(u))
            .WithMessage($"Duration unit must be one of: {string.Join(", ", ValidDurationUnits)}")
            .When(x => x.DurationUnit != null);

        RuleFor(x => x.Deadline)
            .GreaterThan(DateTime.UtcNow).WithMessage("Deadline must be in the future")
            .When(x => x.Deadline.HasValue);
    }
}
