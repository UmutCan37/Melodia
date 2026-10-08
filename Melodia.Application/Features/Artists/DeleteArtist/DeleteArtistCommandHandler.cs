using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Albums;
using Melodia.Domain.Artists;
using Melodia.Domain.Songs;

namespace Melodia.Application.Features.Artists.DeleteArtist;

internal sealed class DeleteArtistCommandHandler(
    IArtistRepository artistRepository,
    ISongRepository songRepository,
    IAlbumRepository albumRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteArtistCommand>
{
    public async Task<Result> Handle(DeleteArtistCommand request, CancellationToken cancellationToken)
    {
        var artist = await artistRepository.GetByIdAsync(request.ArtistId, cancellationToken);
        if (artist is null)
            return ArtistErrors.NotFound(request.ArtistId);

        if (await songRepository.ExistsByArtistAsync(artist.Id, cancellationToken))
            return ArtistErrors.HasSongs;

        if (await albumRepository.ExistsByArtistAsync(artist.Id, cancellationToken))
            return ArtistErrors.HasAlbums;

        artistRepository.Remove(artist);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}