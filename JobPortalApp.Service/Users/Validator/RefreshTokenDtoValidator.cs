using FluentValidation;
using JobPortalApp.Shared.Security.Dtos;

namespace JobPortalApp.Service.Users.Validator;

public class RefreshTokenDtoValidator : AbstractValidator<RefreshTokenDto>
{
    public RefreshTokenDtoValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}
