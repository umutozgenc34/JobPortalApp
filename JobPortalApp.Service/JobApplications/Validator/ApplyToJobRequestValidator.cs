using FluentValidation;
using JobPortalApp.Model.JobApplications.Dtos;

namespace JobPortalApp.Service.JobApplications.Validator;

public class ApplyToJobRequestValidator : AbstractValidator<ApplyToJobRequest>
{
    public ApplyToJobRequestValidator()
    {
        RuleFor(x => x.JobPostingId)
            .NotEmpty().WithMessage("Job posting ID is required.");

        RuleFor(x => x.CoverLetter)
            .MaximumLength(1000).WithMessage("Cover letter cannot exceed 1000 characters.");
    }
}