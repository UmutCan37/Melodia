using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Artists;

namespace Melodia.Application.Features.Artists.UpdateArtist;

internal sealed class UpdateArtistCommandHandler(
    IArtistRepository artistRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateArtistCommand>
{
    public async Task<Result> Handle(UpdateArtistCommand request, CancellationToken cancellationToken)
    {
        var artist = await artistRepository.GetByIdAsync(request.ArtistId, cancellationToken);
        if (artist is null)
            return ArtistErrors.NotFound(request.ArtistId);

        var updateResult = artist.UpdateProfile(request.Name, request.Biography);
        if (updateResult.IsFailure)
            return updateResult.Error;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}