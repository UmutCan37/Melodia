using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Playlists;

namespace Melodia.Application.Features.Playlists.RemoveSongFromPlaylist;

internal sealed class RemoveSongFromPlaylistCommandHandler(
    IUserContext userContext,
    IPlaylistRepository playlistRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveSongFromPlaylistCommand>
{
    public async Task<Result> Handle(RemoveSongFromPlaylistCommand request, CancellationToken cancellationToken)
    {
        var playlist = await playlistRepository.GetByIdAsync(request.PlaylistId, cancellationToken);
        if (playlist is null)
            return PlaylistErrors.NotFound(request.PlaylistId);

        // Sahiplik, "listede var mı" ve sıra düzeltme Domain'de
        var removeResult = playlist.RemoveSong(userContext.UserId, request.SongId);
        if (removeResult.IsFailure)
            return removeResult.Error;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}