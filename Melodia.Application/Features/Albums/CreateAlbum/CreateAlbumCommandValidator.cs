using FluentValidation;
using Melodia.Domain.Albums;

namespace Melodia.Application.Features.Albums.CreateAlbum;

internal sealed class CreateAlbumCommandValidator : AbstractValidator<CreateAlbumCommand>
{
    public CreateAlbumCommandValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("Albüm adı boş olamaz.")
            .MaximumLength(Album.TitleMaxLength).WithMessage($"Albüm adı en fazla {Album.TitleMaxLength} karakter olabilir.");

        RuleFor(c => c.ArtistId)
            .NotEmpty().WithMessage("Sanatçı Id'si boş olamaz.");

        RuleFor(c => c.ReleaseDate)
            .NotEmpty().WithMessage("Çıkış tarihi boş olamaz.");
    }
}