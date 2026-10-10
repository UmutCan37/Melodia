using FluentValidation;
using Melodia.Application.Common;
using Melodia.Domain.Albums;

namespace Melodia.Application.Features.Albums.GetAlbums;

internal sealed class GetAlbumsQueryValidator : AbstractValidator<GetAlbumsQuery>
{
    public GetAlbumsQueryValidator()
    {
        RuleFor(q => q.Page).ValidPage();
        RuleFor(q => q.PageSize).ValidPageSize();

        RuleFor(q => q.Search)
            .MaximumLength(Album.TitleMaxLength).WithMessage($"Arama metni en fazla {Album.TitleMaxLength} karakter olabilir.");
    }
}