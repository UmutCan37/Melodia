using Melodia.Application.Abstractions.Data;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Songs;
using Microsoft.EntityFrameworkCore;

namespace Melodia.Application.Features.Songs.GetSongById;

internal sealed class GetSongByIdQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetSongByIdQuery, SongResponse>
{
    public async Task<Result<SongResponse>> Handle(GetSongByIdQuery request, CancellationToken cancellationToken)
    {
        var song = await (
            from s in context.Songs.AsNoTracking()
            where s.Id == request.SongId
            join artist in context.Artists on s.ArtistId equals artist.Id
            join genre in context.Genres on s.GenreId equals genre.Id
            join album in context.Albums on s.AlbumId equals album.Id into albumGroup
            from album in albumGroup.DefaultIfEmpty()
            select new SongResponse(
                s.Id,
                s.Title,
                s.Duration.TotalSeconds,
                artist.Id,
                artist.Name,
                genre.Id,
                genre.Name,
                s.AlbumId,
                album != null ? album.Title : null,
                s.RequiredPackage.Name))
            .FirstOrDefaultAsync(cancellationToken);

        if (song is null)
            return SongErrors.NotFound(request.SongId);

        return song;
    }
}