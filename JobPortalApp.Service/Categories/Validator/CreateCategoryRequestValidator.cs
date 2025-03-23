using FluentValidation;
using JobPortalApp.Model.Categories.Dtos;

namespace JobPortalApp.Service.Categories.Validator;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(256)
            .WithMessage("Name must not exceed 256 characters.");
    }
}