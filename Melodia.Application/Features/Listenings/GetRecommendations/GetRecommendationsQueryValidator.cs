using FluentValidation;

namespace Melodia.Application.Features.Listenings.GetRecommendations;

internal sealed class GetRecommendationsQueryValidator : AbstractValidator<GetRecommendationsQuery>
{
    public GetRecommendationsQueryValidator()
    {
        RuleFor(q => q.Limit)
            .InclusiveBetween(1, 50).WithMessage("Limit 1 ile 50 arasında olmalıdır.");
    }
}