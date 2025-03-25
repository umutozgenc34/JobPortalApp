using FluentValidation;
using JobPortalApp.Model.Companies.Dtos;

namespace JobPortalApp.Service.Companies.Validator;

public class UpdateCompanyRequestValidator : AbstractValidator<UpdateCompanyRequest>
{
    public UpdateCompanyRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Please enter a valid ID.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Company name cannot be empty.")
            .MaximumLength(100).WithMessage("Company name must be at most 100 characters.");

        RuleFor(x => x.About)
            .NotEmpty().WithMessage("About section cannot be empty.")
            .MaximumLength(500).WithMessage("About section must be at most 500 characters.");

        RuleFor(x => x.Industry)
            .NotEmpty().WithMessage("Industry cannot be empty.")
            .MaximumLength(50).WithMessage("Industry must be at most 50 characters.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Please enter a valid email address.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?\d{10,15}$").WithMessage("Please enter a valid phone number.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}