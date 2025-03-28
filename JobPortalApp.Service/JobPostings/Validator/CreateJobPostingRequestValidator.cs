using FluentValidation;
using JobPortalApp.Model.JobPostings.Dtos;

namespace JobPortalApp.Service.JobPostings.Validator;

public class CreateJobPostingRequestValidator : AbstractValidator<CreateJobPostingRequest>
{
    public CreateJobPostingRequestValidator()
    {
        RuleFor(x => x.Title)
          .NotEmpty().WithMessage("Job title is required.")
          .MaximumLength(255).WithMessage("Job title must not exceed 255 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Job description is required.");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.")
            .MaximumLength(150).WithMessage("Location must not exceed 150 characters.");

        RuleFor(x => x.SalaryRange)
            .MaximumLength(50).WithMessage("Salary range must not exceed 50 characters.");

        RuleFor(x => x.CompanyId)
            .GreaterThan(0).WithMessage("CompanyId must be greater than 0.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId must be greater than 0.");
    }
}
