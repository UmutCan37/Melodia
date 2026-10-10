using FluentValidation;
using Melodia.Domain.Shared;

namespace Melodia.Application.Features.Listeners.ChangePackage;

internal sealed class ChangePackageCommandValidator : AbstractValidator<ChangePackageCommand>
{
    public ChangePackageCommandValidator()
    {
        RuleFor(c => c.Package)
            .NotEmpty().WithMessage("Paket boş olamaz.")
            .Must(p => Package.FromName(p).IsSuccess).WithMessage("Geçersiz paket. (Basic, Gold, Premium, Elit)");
    }
}