using FluentValidation;
using JobPortalApp.Model.Users.Dtos;
using JobPortalApp.Service.Validators;

namespace JobPortalApp.Service.Users.Validator;

public class UpdateUserProfileRequestValidator : AbstractValidator<UpdateUserProfileRequest>
{
    public UpdateUserProfileRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name must be at most 100 characters.");

        RuleFor(x => x.Summary)
            .NotEmpty().WithMessage("Summary is required.")
            .MaximumLength(500).WithMessage("Summary must be at most 500 characters.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email address.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.ProfileImage)
            .Must(FileRules.IsImage).WithMessage("Only image files are allowed (jpg, jpeg, png, webp).")
            .Must(file => file!.Length <= FileRules.MaxImageSizeInBytes).WithMessage("Profile image must be smaller than 2 MB.")
            .When(x => x.ProfileImage is not null);

        RuleFor(x => x.CvFile)
            .Must(FileRules.IsPdf).WithMessage("Only PDF files are allowed.")
            .Must(file => file!.Length <= FileRules.MaxPdfSizeInBytes).WithMessage("CV file must be smaller than 5 MB.")
            .When(x => x.CvFile is not null);
    }

}

