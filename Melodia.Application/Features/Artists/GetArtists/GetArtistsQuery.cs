using Melodia.Application.Abstractions.Messaging;
using Melodia.Application.Common;

namespace Melodia.Application.Features.Artists.GetArtists;

public sealed record GetArtistsQuery(
    string? Search,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResponse<ArtistSummaryResponse>>;