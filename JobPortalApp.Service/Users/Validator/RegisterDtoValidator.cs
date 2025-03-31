using FluentValidation;
using JobPortalApp.Shared.Security.Dtos;

namespace JobPortalApp.Service.Users.Validator;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required.")
                .Length(3, 50).WithMessage("UserName must be between 3 and 50 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Enter a valid email address.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password cannot be empty.")
            .Length(6, 100).WithMessage("Password must be between 6 and 100 characters.");
    }
}