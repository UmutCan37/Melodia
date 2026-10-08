using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Artists;

namespace Melodia.Application.Features.Artists.CreateArtist;

internal sealed class CreateArtistCommandHandler(
    IArtistRepository artistRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateArtistCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateArtistCommand request, CancellationToken cancellationToken)
    {
        var artistResult = Artist.Create(request.Name, request.Biography);
        if (artistResult.IsFailure)
            return artistResult.Error;

        artistRepository.Add(artistResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return artistResult.Value.Id;
    }
}