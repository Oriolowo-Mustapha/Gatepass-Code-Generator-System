using FluentValidation;

namespace Application.Features.GatepassRequests.Commands.CreateGatepassRequest;

public class CreateGatepassRequestCommandValidator : AbstractValidator<CreateGatepassRequestCommand>
{
    public CreateGatepassRequestCommandValidator()
    {
        RuleFor(x => x.VisitorFirstName)
            .NotEmpty().WithMessage("Visitor First Name is required.")
            .MaximumLength(50).WithMessage("Visitor First Name must not exceed 50 characters.");

        RuleFor(x => x.VisitorLastName)
            .NotEmpty().WithMessage("Visitor Last Name is required.")
            .MaximumLength(50).WithMessage("Visitor Last Name must not exceed 50 characters.");

        RuleFor(x => x.VisitorContactNumber)
            .NotEmpty().WithMessage("Visitor Contact Number is required.")
            .MaximumLength(20).WithMessage("Visitor Contact Number must not exceed 20 characters.");

        RuleFor(x => x.VisitorEmail)
            .NotEmpty().WithMessage("Visitor Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

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
