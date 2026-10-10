using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Albums;
using Melodia.Domain.Songs;

namespace Melodia.Application.Features.Songs.AssignSongToAlbum;

internal sealed class AssignSongToAlbumCommandHandler(
    ISongRepository songRepository,
    IAlbumRepository albumRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AssignSongToAlbumCommand>
{
    public async Task<Result> Handle(AssignSongToAlbumCommand request, CancellationToken cancellationToken)
    {
        var song = await songRepository.GetByIdAsync(request.SongId, cancellationToken);
        if (song is null)
            return SongErrors.NotFound(request.SongId);

        var album = await albumRepository.GetByIdAsync(request.AlbumId, cancellationToken);
        if (album is null)
            return AlbumErrors.NotFound(request.AlbumId);

        var assignResult = song.AssignToAlbum(album);
        if (assignResult.IsFailure)
            return assignResult.Error;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}