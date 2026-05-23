using FluentValidation;
using FreelanceHub.Application.DTOs.Bids;

namespace FreelanceHub.Application.Validators;

public class CreateBidRequestValidator : AbstractValidator<CreateBidRequest>
{
    private static readonly string[] ValidDeliveryUnits = { "Hours", "Days", "Weeks", "Months" };

    public CreateBidRequestValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Bid amount must be greater than 0")
            .LessThanOrEqualTo(1_000_000).WithMessage("Bid amount cannot exceed 1,000,000");

        RuleFor(x => x.DeliveryTime)
            .GreaterThan(0).WithMessage("Delivery time must be greater than 0");

        RuleFor(x => x.DeliveryTimeUnit)
            .Must(u => ValidDeliveryUnits.Contains(u))
            .WithMessage($"Delivery time unit must be one of: {string.Join(", ", ValidDeliveryUnits)}");

        RuleFor(x => x.CoverLetter)
            .NotEmpty().WithMessage("Cover letter is required")
            .MinimumLength(100).WithMessage("Cover letter must be at least 100 characters")
            .MaximumLength(2000).WithMessage("Cover letter cannot exceed 2000 characters");
    }
}
