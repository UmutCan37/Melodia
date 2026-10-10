using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Abstractions.Data;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Playlists;
using Microsoft.EntityFrameworkCore;

namespace Melodia.Application.Features.Playlists.GetPlaylistById;

internal sealed class GetPlaylistByIdQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext) : IQueryHandler<GetPlaylistByIdQuery, PlaylistResponse>
{
    public async Task<Result<PlaylistResponse>> Handle(GetPlaylistByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Liste var mı, kimin?
        var playlist = await context.Playlists
            .AsNoTracking()
            .Where(p => p.Id == request.PlaylistId)
            .Select(p => new { p.Id, p.Name, p.OwnerId })
            .FirstOrDefaultAsync(cancellationToken);

        if (playlist is null)
            return PlaylistErrors.NotFound(request.PlaylistId);

        if (playlist.OwnerId != userContext.UserId)
            return PlaylistErrors.NotOwner;

        // 2. Şarkılar (yayından kaldırılanlar dahil)
        var songs = await (
            from ps in context.Playlists
                .Where(p => p.Id == playlist.Id)
                .SelectMany(p => p.Songs)
            join song in context.Songs.IgnoreQueryFilters() on ps.SongId equals song.Id
            join artist in context.Artists on song.ArtistId equals artist.Id
            orderby ps.Position
            select new PlaylistSongResponse(
                song.Id,
                ps.Position,
                song.Title,
                artist.Name,
                song.Duration.TotalSeconds,
                song.RequiredPackage.Name,
                ps.AddedAtUtc,
                !song.IsDeleted))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new PlaylistResponse(playlist.Id, playlist.Name, songs);
    }
}