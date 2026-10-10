using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Listenings.GetRecommendations;

public sealed record GetRecommendationsQuery(int Limit = 20) : IQuery<IReadOnlyList<RecommendationResponse>>;