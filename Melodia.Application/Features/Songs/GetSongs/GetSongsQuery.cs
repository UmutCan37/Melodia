using Melodia.Application.Abstractions.Messaging;
using Melodia.Application.Common;

namespace Melodia.Application.Features.Songs.GetSongs;

public sealed record GetSongsQuery(
    string? Search,
    Guid? ArtistId,
    Guid? GenreId,
    Guid? AlbumId,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResponse<SongSummaryResponse>>;