using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Commands.RegisterUser
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator() {

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress().WithMessage("Invalid Email Format");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password Required!! ")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

        }
    }
}
