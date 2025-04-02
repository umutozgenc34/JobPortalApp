using FluentValidation;
using JobPortalApp.Model.Companies.Dtos;

namespace JobPortalApp.Service.Companies.Validator;

public class UpdateCompanyReviewRequestValidator : AbstractValidator<UpdateCompanyReviewRequest>
{
    public UpdateCompanyReviewRequestValidator()
    {
        RuleFor(x => x.Rating)
           .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Comment is required.")
            .MaximumLength(500).WithMessage("Comment cannot be longer than 500 characters.");
    }
}
