using FluentValidation;

namespace Application.Features.GatepassRequests.Commands.CreateGatepassRequest;

public class CreateGatepassRequestCommandValidator : AbstractValidator<CreateGatepassRequestCommand>
{
    public CreateGatepassRequestCommandValidator()
    {
        RuleFor(x => x.VisitorId)
            .NotEmpty().WithMessage("Visitor ID is required.");

        RuleFor(x => x.HostUserId)
            .NotEmpty().WithMessage("Host User ID is required.");

        RuleFor(x => x.VisitPurpose)
            .NotEmpty().WithMessage("Visit purpose is required.")
            .MaximumLength(500).WithMessage("Visit purpose must not exceed 500 characters.");

        RuleFor(x => x.ValidFrom)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Valid From date cannot be in the past.");

        RuleFor(x => x.ValidUntil)
            .GreaterThan(x => x.ValidFrom)
            .WithMessage("Valid Until must be after Valid From.");
    }
}
