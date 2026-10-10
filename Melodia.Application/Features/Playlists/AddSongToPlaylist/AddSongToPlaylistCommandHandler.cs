using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Listeners;
using Melodia.Domain.Playlists;
using Melodia.Domain.Songs;

namespace Melodia.Application.Features.Playlists.AddSongToPlaylist;

internal sealed class AddSongToPlaylistCommandHandler(
    IUserContext userContext,
    IPlaylistRepository playlistRepository,
    IListenerRepository listenerRepository,
    ISongRepository songRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<AddSongToPlaylistCommand>
{
    public async Task<Result> Handle(AddSongToPlaylistCommand request, CancellationToken cancellationToken)
    {
        var playlist = await playlistRepository.GetByIdAsync(request.PlaylistId, cancellationToken);
        if (playlist is null)
            return PlaylistErrors.NotFound(request.PlaylistId);

        var listener = await listenerRepository.GetByIdAsync(userContext.UserId, cancellationToken);
        if (listener is null)
            return ListenerErrors.NotFound(userContext.UserId);

        var song = await songRepository.GetByIdAsync(request.SongId, cancellationToken);
        if (song is null)
            return SongErrors.NotFound(request.SongId);

        // Sahiplik, paket ve tekrar kontrolü Domain'de
        var addResult = playlist.AddSong(listener, song, timeProvider.GetUtcNow().UtcDateTime);
        if (addResult.IsFailure)
            return addResult.Error;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}