using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Albums;
using Melodia.Domain.Artists;

namespace Melodia.Application.Features.Albums.CreateAlbum;

internal sealed class CreateAlbumCommandHandler(
    IAlbumRepository albumRepository,
    IArtistRepository artistRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateAlbumCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateAlbumCommand request, CancellationToken cancellationToken)
    {
        if (!await artistRepository.ExistsAsync(request.ArtistId, cancellationToken))
            return ArtistErrors.NotFound(request.ArtistId);

        var albumResult = Album.Create(request.Title, request.ArtistId, request.ReleaseDate);
        if (albumResult.IsFailure)
            return albumResult.Error;

        albumRepository.Add(albumResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return albumResult.Value.Id;
    }
}