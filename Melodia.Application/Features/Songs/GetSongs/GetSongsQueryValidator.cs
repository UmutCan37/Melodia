using FluentValidation;
using Melodia.Application.Common;
using Melodia.Domain.Songs;

namespace Melodia.Application.Features.Songs.GetSongs;

internal sealed class GetSongsQueryValidator : AbstractValidator<GetSongsQuery>
{
    public GetSongsQueryValidator()
    {
        RuleFor(q => q.Page).ValidPage();
        RuleFor(q => q.PageSize).ValidPageSize();

        RuleFor(q => q.Search)
            .MaximumLength(Song.TitleMaxLength).WithMessage($"Arama metni en fazla {Song.TitleMaxLength} karakter olabilir.");
    }
}