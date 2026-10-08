using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Albums;

namespace Melodia.Application.Features.Albums.UpdateAlbum;

internal sealed class UpdateAlbumCommandHandler(
    IAlbumRepository albumRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateAlbumCommand>
{
    public async Task<Result> Handle(UpdateAlbumCommand request, CancellationToken cancellationToken)
    {
        var album = await albumRepository.GetByIdAsync(request.AlbumId, cancellationToken);
        if (album is null)
            return AlbumErrors.NotFound(request.AlbumId);

        var updateResult = album.UpdateDetails(request.Title, request.ReleaseDate);
        if (updateResult.IsFailure)
            return updateResult.Error;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}