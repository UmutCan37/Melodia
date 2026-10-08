using Melodia.Application.Abstractions.Messaging;
using Melodia.Application.Abstractions.Storage;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Albums;
using Melodia.Domain.Artists;
using Melodia.Domain.Genres;
using Melodia.Domain.Shared;
using Melodia.Domain.Songs;

namespace Melodia.Application.Features.Songs.CreateSong;

internal sealed class CreateSongCommandHandler(
    ISongRepository songRepository,
    IArtistRepository artistRepository,
    IGenreRepository genreRepository,
    IAlbumRepository albumRepository,
    IAudioFileStorage audioFileStorage,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateSongCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateSongCommand request, CancellationToken cancellationToken)
    {
        // 1. Dosyaya dokunmadan önce yapılabilecek bütün kontroller
        if (!await artistRepository.ExistsAsync(request.ArtistId, cancellationToken))
            return ArtistErrors.NotFound(request.ArtistId);

        if (!await genreRepository.ExistsAsync(request.GenreId, cancellationToken))
            return GenreErrors.NotFound(request.GenreId);

        Album? album = null;
        if (request.AlbumId is not null)
        {
            album = await albumRepository.GetByIdAsync(request.AlbumId.Value, cancellationToken);
            if (album is null)
                return AlbumErrors.NotFound(request.AlbumId.Value);
        }

        var durationResult = Duration.FromSeconds(request.DurationSeconds);
        if (durationResult.IsFailure)
            return durationResult.Error;

        var packageResult = Package.FromName(request.RequiredPackage);
        if (packageResult.IsFailure)
            return packageResult.Error;

        // 2. Dosyayı kaydet
        var filePath = await audioFileStorage.SaveAsync(request.AudioContent, request.AudioFileName, cancellationToken);

        // 3. Buradan sonra bir şey ters giderse dosya diskte sahipsiz kalmasın
        try
        {
            var audioFileResult = AudioFile.Create(filePath);
            if (audioFileResult.IsFailure)
                return await DeleteFileAndFail(filePath, audioFileResult.Error);

            var songResult = Song.Create(
                request.Title,
                durationResult.Value,
                request.ArtistId,
                request.GenreId,
                audioFileResult.Value,
                packageResult.Value);

            if (songResult.IsFailure)
                return await DeleteFileAndFail(filePath, songResult.Error);

            var song = songResult.Value;

            if (album is not null)
            {
                var assignResult = song.AssignToAlbum(album);
                if (assignResult.IsFailure)
                    return await DeleteFileAndFail(filePath, assignResult.Error);
            }

            songRepository.Add(song);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return song.Id;
        }
        catch
        {
            await audioFileStorage.DeleteAsync(filePath, CancellationToken.None);
            throw;
        }
    }

    private async Task<Result<Guid>> DeleteFileAndFail(string filePath, Error error)
    {
        await audioFileStorage.DeleteAsync(filePath, CancellationToken.None);
        return error;
    }
}