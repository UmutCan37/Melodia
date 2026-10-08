using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Albums;
using Melodia.Domain.Songs;

namespace Melodia.Application.Features.Albums.DeleteAlbum;

internal sealed class DeleteAlbumCommandHandler(
    IAlbumRepository albumRepository,
    ISongRepository songRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteAlbumCommand>
{
    public async Task<Result> Handle(DeleteAlbumCommand request, CancellationToken cancellationToken)
    {
        var album = await albumRepository.GetByIdAsync(request.AlbumId, cancellationToken);
        if (album is null)
            return AlbumErrors.NotFound(request.AlbumId);

        if (await songRepository.ExistsByAlbumAsync(album.Id, cancellationToken))
            return AlbumErrors.HasSongs;

        albumRepository.Remove(album);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}