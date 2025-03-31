using FluentValidation;
using JobPortalApp.Model.Users.Dtos;

namespace JobPortalApp.Service.Users.Validator;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.UserName)
              .NotEmpty().WithMessage("UserName is required.")
              .Length(3, 50).WithMessage("Username must be between 3 and 50 characters.");
    }

}