using FluentValidation;
using Melodia.Domain.Listeners;
using Melodia.Domain.Shared;

namespace Melodia.Application.Features.Auth.Register;

internal sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("E-posta boş olamaz.")
            .Must(e => Email.Create(e).IsSuccess).WithMessage("Geçerli bir e-posta adresi girin.");

        RuleFor(c => c.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz.")
            .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
            .MaximumLength(128).WithMessage("Şifre en fazla 128 karakter olabilir.")
            .Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
            .Matches("[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
            .Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir.");

        RuleFor(c => c.DisplayName)
            .NotEmpty().WithMessage("Görünen ad boş olamaz.")
            .MaximumLength(Listener.DisplayNameMaxLength).WithMessage($"Görünen ad en fazla {Listener.DisplayNameMaxLength} karakter olabilir.");
    }
}