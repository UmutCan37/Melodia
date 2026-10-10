using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Playlists;

namespace Melodia.Application.Features.Playlists.RenamePlaylist;

internal sealed class RenamePlaylistCommandHandler(
    IUserContext userContext,
    IPlaylistRepository playlistRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RenamePlaylistCommand>
{
    public async Task<Result> Handle(RenamePlaylistCommand request, CancellationToken cancellationToken)
    {
        var playlist = await playlistRepository.GetByIdAsync(request.PlaylistId, cancellationToken);
        if (playlist is null)
            return PlaylistErrors.NotFound(request.PlaylistId);

        // Sahiplik kontrolü Domain'de: başkasının listesiyse NotOwner (403)
        var renameResult = playlist.Rename(userContext.UserId, request.Name);
        if (renameResult.IsFailure)
            return renameResult.Error;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}