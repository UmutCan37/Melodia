using FluentValidation;
using Melodia.Domain.Albums;

namespace Melodia.Application.Features.Albums.UpdateAlbum;

internal sealed class UpdateAlbumCommandValidator : AbstractValidator<UpdateAlbumCommand>
{
    public UpdateAlbumCommandValidator()
    {
        RuleFor(c => c.AlbumId)
            .NotEmpty().WithMessage("Albüm Id'si boş olamaz.");

        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("Albüm adı boş olamaz.")
            .MaximumLength(Album.TitleMaxLength).WithMessage($"Albüm adı en fazla {Album.TitleMaxLength} karakter olabilir.");

        RuleFor(c => c.ReleaseDate)
            .NotEmpty().WithMessage("Çıkış tarihi boş olamaz.");
    }
}