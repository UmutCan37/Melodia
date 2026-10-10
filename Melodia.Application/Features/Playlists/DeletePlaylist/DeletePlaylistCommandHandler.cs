using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Playlists;

namespace Melodia.Application.Features.Playlists.DeletePlaylist;

internal sealed class DeletePlaylistCommandHandler(
    IUserContext userContext,
    IPlaylistRepository playlistRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeletePlaylistCommand>
{
    public async Task<Result> Handle(DeletePlaylistCommand request, CancellationToken cancellationToken)
    {
        var playlist = await playlistRepository.GetByIdAsync(request.PlaylistId, cancellationToken);
        if (playlist is null)
            return PlaylistErrors.NotFound(request.PlaylistId);

        var ownerResult = playlist.EnsureOwner(userContext.UserId);
        if (ownerResult.IsFailure)
            return ownerResult.Error;

        playlistRepository.Remove(playlist);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}