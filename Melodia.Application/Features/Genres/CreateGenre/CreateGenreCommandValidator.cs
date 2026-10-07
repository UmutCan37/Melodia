using FluentValidation;
using Melodia.Domain.Genres;

namespace Melodia.Application.Features.Genres.CreateGenre;

internal sealed class CreateGenreCommandValidator : AbstractValidator<CreateGenreCommand>
{
    public CreateGenreCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Tür adı boş olamaz.")
            .MaximumLength(Genre.NameMaxLength).WithMessage($"Tür adı en fazla {Genre.NameMaxLength} karakter olabilir.");
    }
}