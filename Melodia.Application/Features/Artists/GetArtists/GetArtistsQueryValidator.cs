using FluentValidation;
using Melodia.Application.Common;
using Melodia.Domain.Artists;

namespace Melodia.Application.Features.Artists.GetArtists;

internal sealed class GetArtistsQueryValidator : AbstractValidator<GetArtistsQuery>
{
    public GetArtistsQueryValidator()
    {
        RuleFor(q => q.Page).ValidPage();
        RuleFor(q => q.PageSize).ValidPageSize();

        RuleFor(q => q.Search)
            .MaximumLength(Artist.NameMaxLength).WithMessage($"Arama metni en fazla {Artist.NameMaxLength} karakter olabilir.");
    }
}