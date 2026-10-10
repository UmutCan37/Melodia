using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Songs;

namespace Melodia.Application.Features.Songs.DeleteSong;

internal sealed class DeleteSongCommandHandler(
    ISongRepository songRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<DeleteSongCommand>
{
    public async Task<Result> Handle(DeleteSongCommand request, CancellationToken cancellationToken)
    {
        var song = await songRepository.GetByIdAsync(request.SongId, cancellationToken);
        if (song is null)
            return SongErrors.NotFound(request.SongId);

        song.Delete(timeProvider.GetUtcNow().UtcDateTime);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}