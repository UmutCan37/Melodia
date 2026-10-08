using FluentValidation;
using Melodia.Domain.Albums;

namespace Melodia.Application.Features.Albums.GetAlbums;

internal sealed class GetAlbumsQueryValidator : AbstractValidator<GetAlbumsQuery>
{
    public GetAlbumsQueryValidator()
    {
        RuleFor(q => q.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Sayfa numarası en az 1 olmalıdır.");

        RuleFor(q => q.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Sayfa boyutu 1 ile 100 arasında olmalıdır.");

        RuleFor(q => q.Search)
            .MaximumLength(Album.TitleMaxLength).WithMessage($"Arama metni en fazla {Album.TitleMaxLength} karakter olabilir.");
    }
}