using FluentValidation;

namespace Melodia.Application.Features.Listenings.GetRecentlyPlayed;

internal sealed class GetRecentlyPlayedQueryValidator : AbstractValidator<GetRecentlyPlayedQuery>
{
    public GetRecentlyPlayedQueryValidator()
    {
        RuleFor(q => q.Limit)
            .InclusiveBetween(1, 50).WithMessage("Limit 1 ile 50 arasında olmalıdır.");
    }
}