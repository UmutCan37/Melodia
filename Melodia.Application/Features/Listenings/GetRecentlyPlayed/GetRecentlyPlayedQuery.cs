using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Listenings.GetRecentlyPlayed;

public sealed record GetRecentlyPlayedQuery(int Limit = 20) : IQuery<IReadOnlyList<RecentlyPlayedResponse>>;