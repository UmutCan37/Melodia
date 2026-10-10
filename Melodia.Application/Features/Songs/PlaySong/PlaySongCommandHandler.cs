using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Application.Abstractions.Storage;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Listeners;
using Melodia.Domain.Listenings;
using Melodia.Domain.Services;
using Melodia.Domain.Songs;

namespace Melodia.Application.Features.Songs.PlaySong;

internal sealed class PlaySongCommandHandler(
    IUserContext userContext,
    IListenerRepository listenerRepository,
    ISongRepository songRepository,
    IListeningRecordRepository listeningRecordRepository,
    IStreamUrlSigner streamUrlSigner,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<PlaySongCommand, PlaySongResponse>
{
    public async Task<Result<PlaySongResponse>> Handle(PlaySongCommand request, CancellationToken cancellationToken)
    {
        // 1. Kim çalıyor? (token'dan, güncel paketiyle veritabanından)
        var listener = await listenerRepository.GetByIdAsync(userContext.UserId, cancellationToken);
        if (listener is null)
            return ListenerErrors.NotFound(userContext.UserId);

        // 2. Ne çalınıyor?
        var song = await songRepository.GetByIdAsync(request.SongId, cancellationToken);
        if (song is null)
            return SongErrors.NotFound(request.SongId);

        // 3. Projenin kalbi: paketi yetiyor mu?
        var canPlay = PlaybackPolicy.CanPlay(listener, song);
        if (canPlay.IsFailure)
            return canPlay.Error;

        // 4. Dinlemeyi kaydet (SongListenedEvent burada bırakılıyor)
        var record = ListeningRecord.Create(listener.Id, song.Id, timeProvider.GetUtcNow().UtcDateTime);
        listeningRecordRepository.Add(record);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Kısa ömürlü, imzalı link
        var streamUrl = streamUrlSigner.CreateSignedUrl(song.Id);

        return new PlaySongResponse(song.Id, streamUrl);
    }
}