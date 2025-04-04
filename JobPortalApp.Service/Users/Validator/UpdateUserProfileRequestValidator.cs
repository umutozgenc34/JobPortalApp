using FluentValidation;
using JobPortalApp.Model.Users.Dtos;
using Microsoft.AspNetCore.Http;

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
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");

        RuleFor(x => x.ProfileImage)
            .Must(BeAnImageFile).When(x => x.ProfileImage is not null)
            .WithMessage("Only image files are allowed (jpg, jpeg, png).");

        RuleFor(x => x.CvFile)
            .Must(BeAPdfFile).When(x => x.CvFile is not null)
            .WithMessage("Only PDF files are allowed.");
    }

    private bool BeAnImageFile(IFormFile file)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return allowedExtensions.Contains(extension);
    }

    private bool BeAPdfFile(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return extension == ".pdf";
    }

}

