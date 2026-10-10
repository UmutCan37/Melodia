using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Playlists;

namespace Melodia.Application.Features.Playlists.CreatePlaylist;

internal sealed class CreatePlaylistCommandHandler(
    IUserContext userContext,
    IPlaylistRepository playlistRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreatePlaylistCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreatePlaylistCommand request, CancellationToken cancellationToken)
    {
        var playlistResult = Playlist.Create(userContext.UserId, request.Name);
        if (playlistResult.IsFailure)
            return playlistResult.Error;

        playlistRepository.Add(playlistResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return playlistResult.Value.Id;
    }
}