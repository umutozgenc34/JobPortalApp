using FluentValidation;
using JobPortalApp.Shared.Security.Dtos;

namespace JobPortalApp.Service.Users.Validator;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("Invalid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password cannot be empty.");
    }
}