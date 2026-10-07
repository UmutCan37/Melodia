using FluentValidation;
using Melodia.Domain.Genres;

namespace Melodia.Application.Features.Genres.RenameGenre;

internal sealed class RenameGenreCommandValidator : AbstractValidator<RenameGenreCommand>
{
    public RenameGenreCommandValidator()
    {
        RuleFor(c => c.GenreId)
            .NotEmpty().WithMessage("Tür Id'si boş olamaz.");

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Tür adı boş olamaz.")
            .MaximumLength(Genre.NameMaxLength).WithMessage($"Tür adı en fazla {Genre.NameMaxLength} karakter olabilir.");
    }
}