using Melodia.Application.Abstractions.Data;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Application.Common;
using Melodia.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Melodia.Application.Features.Songs.GetSongs;

internal sealed class GetSongsQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetSongsQuery, PagedResponse<SongSummaryResponse>>
{
    public async Task<Result<PagedResponse<SongSummaryResponse>>> Handle(
        GetSongsQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Filtreler sadece Songs üzerinde
        var songs = context.Songs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            songs = songs.Where(s => s.Title.Contains(search));
        }

        if (request.ArtistId is not null)
            songs = songs.Where(s => s.ArtistId == request.ArtistId);

        if (request.GenreId is not null)
            songs = songs.Where(s => s.GenreId == request.GenreId);

        if (request.AlbumId is not null)
            songs = songs.Where(s => s.AlbumId == request.AlbumId);

        var totalCount = await songs.CountAsync(cancellationToken);

        // 2. Sadece bu sayfadaki şarkılar
        var pagedSongs = songs
            .OrderBy(s => s.Title)
            .ThenBy(s => s.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);

        // 3. Sayfadaki şarkıları sanatçı, tür ve (varsa) albümle birleştir
        var items = await (
            from song in pagedSongs
            join artist in context.Artists on song.ArtistId equals artist.Id
            join genre in context.Genres on song.GenreId equals genre.Id
            join album in context.Albums on song.AlbumId equals album.Id into albumGroup
            from album in albumGroup.DefaultIfEmpty()
            orderby song.Title, song.Id
            select new SongSummaryResponse(
                song.Id,
                song.Title,
                song.Duration.TotalSeconds,
                artist.Id,
                artist.Name,
                genre.Id,
                genre.Name,
                song.AlbumId,
                album != null ? album.Title : null,
                song.RequiredPackage.Name))
            .ToListAsync(cancellationToken);

        return new PagedResponse<SongSummaryResponse>(items, request.Page, request.PageSize, totalCount);
    }
}