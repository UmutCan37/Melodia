using Melodia.Application.Abstractions.Messaging;
using Melodia.Application.Common;

namespace Melodia.Application.Features.Albums.GetAlbums;

public sealed record GetAlbumsQuery(
    string? Search,
    Guid? ArtistId,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResponse<AlbumSummaryResponse>>;