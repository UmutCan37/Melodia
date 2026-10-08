using FluentValidation;
using Melodia.Domain.Artists;

namespace Melodia.Application.Features.Artists.GetArtists;

internal sealed class GetArtistsQueryValidator : AbstractValidator<GetArtistsQuery>
{
    public GetArtistsQueryValidator()
    {
        RuleFor(q => q.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Sayfa numarası en az 1 olmalıdır.");

        RuleFor(q => q.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Sayfa boyutu 1 ile 100 arasında olmalıdır.");

        RuleFor(q => q.Search)
            .MaximumLength(Artist.NameMaxLength).WithMessage($"Arama metni en fazla {Artist.NameMaxLength} karakter olabilir.");
    }
}