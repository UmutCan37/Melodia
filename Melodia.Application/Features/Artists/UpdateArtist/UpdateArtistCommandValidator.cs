using FluentValidation;
using Melodia.Domain.Artists;

namespace Melodia.Application.Features.Artists.UpdateArtist;

internal sealed class UpdateArtistCommandValidator : AbstractValidator<UpdateArtistCommand>
{
    public UpdateArtistCommandValidator()
    {
        RuleFor(c => c.ArtistId)
            .NotEmpty().WithMessage("Sanatçı Id'si boş olamaz.");

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Sanatçı adı boş olamaz.")
            .MaximumLength(Artist.NameMaxLength).WithMessage($"Sanatçı adı en fazla {Artist.NameMaxLength} karakter olabilir.");

        RuleFor(c => c.Biography)
            .MaximumLength(Artist.BiographyMaxLength).WithMessage($"Biyografi en fazla {Artist.BiographyMaxLength} karakter olabilir.");
    }
}