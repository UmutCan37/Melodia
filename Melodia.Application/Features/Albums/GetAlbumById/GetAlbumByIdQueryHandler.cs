using Melodia.Application.Abstractions.Data;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Albums;
using Microsoft.EntityFrameworkCore;

namespace Melodia.Application.Features.Albums.GetAlbumById;

internal sealed class GetAlbumByIdQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetAlbumByIdQuery, AlbumResponse>
{
    public async Task<Result<AlbumResponse>> Handle(GetAlbumByIdQuery request, CancellationToken cancellationToken)
    {
        var album = await context.Albums
            .AsNoTracking()
            .Where(a => a.Id == request.AlbumId)
            .Join(
                context.Artists,
                album => album.ArtistId,
                artist => artist.Id,
                (album, artist) => new
                {
                    album.Id,
                    album.Title,
                    album.ArtistId,
                    ArtistName = artist.Name,
                    album.ReleaseDate
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (album is null)
            return AlbumErrors.NotFound(request.AlbumId);

        var songs = await context.Songs
            .AsNoTracking()
            .Where(s => s.AlbumId == album.Id)
            .OrderBy(s => s.Title)
            .Select(s => new AlbumSongResponse(
                s.Id,
                s.Title,
                s.Duration.TotalSeconds,
                s.RequiredPackage.Name))
            .ToListAsync(cancellationToken);

        return new AlbumResponse(
            album.Id,
            album.Title,
            album.ArtistId,
            album.ArtistName,
            album.ReleaseDate,
            songs);
    }
}