using FluentValidation;

namespace Melodia.Application.Features.Auth.Login;

internal sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("E-posta boş olamaz.");

        RuleFor(c => c.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz.")
            .MaximumLength(128).WithMessage("Şifre en fazla 128 karakter olabilir.");
    }
}