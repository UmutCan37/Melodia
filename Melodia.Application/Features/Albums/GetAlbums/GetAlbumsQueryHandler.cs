using Melodia.Application.Abstractions.Data;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Application.Common;
using Melodia.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Melodia.Application.Features.Albums.GetAlbums;

internal sealed class GetAlbumsQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetAlbumsQuery, PagedResponse<AlbumSummaryResponse>>
{
    public async Task<Result<PagedResponse<AlbumSummaryResponse>>> Handle(
        GetAlbumsQuery request,
        CancellationToken cancellationToken)
    {
        var albums = context.Albums.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            albums = albums.Where(a => a.Title.Contains(search));
        }

        if (request.ArtistId is not null)
            albums = albums.Where(a => a.ArtistId == request.ArtistId);

        var totalCount = await albums.CountAsync(cancellationToken);

        var items = await albums
            .OrderByDescending(a => a.ReleaseDate)
            .ThenBy(a => a.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Join(
                context.Artists,
                album => album.ArtistId,
                artist => artist.Id,
                (album, artist) => new AlbumSummaryResponse(
                    album.Id,
                    album.Title,
                    album.ArtistId,
                    artist.Name,
                    album.ReleaseDate))
            .ToListAsync(cancellationToken);

        return new PagedResponse<AlbumSummaryResponse>(items, request.Page, request.PageSize, totalCount);
    }
}