using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Shared;
using Melodia.Domain.Songs;

namespace Melodia.Application.Features.Songs.UpdateSong;

internal sealed class UpdateSongCommandHandler(
    ISongRepository songRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateSongCommand>
{
    public async Task<Result> Handle(UpdateSongCommand request, CancellationToken cancellationToken)
    {
        var song = await songRepository.GetByIdAsync(request.SongId, cancellationToken);
        if (song is null)
            return SongErrors.NotFound(request.SongId);

        var packageResult = Package.FromName(request.RequiredPackage);
        if (packageResult.IsFailure)
            return packageResult.Error;

        var renameResult = song.Rename(request.Title);
        if (renameResult.IsFailure)
            return renameResult.Error;

        song.ChangeRequiredPackage(packageResult.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}