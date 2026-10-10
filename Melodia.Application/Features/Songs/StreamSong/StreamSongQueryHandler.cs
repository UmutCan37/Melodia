using Melodia.Application.Abstractions.Data;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Application.Abstractions.Storage;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Songs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Melodia.Application.Features.Songs.StreamSong;

internal sealed class StreamSongQueryHandler(
    IStreamUrlSigner streamUrlSigner,
    IApplicationDbContext context,
    IAudioFileStorage audioFileStorage,
    ILogger<StreamSongQueryHandler> logger) : IQueryHandler<StreamSongQuery, Stream>
{
    public async Task<Result<Stream>> Handle(StreamSongQuery request, CancellationToken cancellationToken)
    {
        
        if (!streamUrlSigner.IsValid(request.SongId, request.Expires, request.Signature))
            return StreamErrors.InvalidLink;

        
        var filePath = await context.Songs
            .AsNoTracking()
            .Where(s => s.Id == request.SongId)
            .Select(s => s.AudioFile.FilePath)
            .FirstOrDefaultAsync(cancellationToken);

        if (filePath is null)
            return SongErrors.NotFound(request.SongId);

        var stream = audioFileStorage.OpenRead(filePath);
        if (stream is null)
        {
            logger.LogError("Ses dosyası diskte bulunamadı. SongId: {SongId} | Yol: {FilePath}", request.SongId, filePath);
            return StreamErrors.AudioFileMissing;
        }

        return stream;
    }
}