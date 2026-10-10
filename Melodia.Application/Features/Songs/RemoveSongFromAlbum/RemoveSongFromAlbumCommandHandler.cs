using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Songs;

namespace Melodia.Application.Features.Songs.RemoveSongFromAlbum;

internal sealed class RemoveSongFromAlbumCommandHandler(
    ISongRepository songRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveSongFromAlbumCommand>
{
    public async Task<Result> Handle(RemoveSongFromAlbumCommand request, CancellationToken cancellationToken)
    {
        var song = await songRepository.GetByIdAsync(request.SongId, cancellationToken);
        if (song is null)
            return SongErrors.NotFound(request.SongId);

        song.RemoveFromAlbum();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}