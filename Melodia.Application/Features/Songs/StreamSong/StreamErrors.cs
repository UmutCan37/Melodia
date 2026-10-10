using Melodia.Domain.Abstractions;

namespace Melodia.Application.Features.Songs.StreamSong;

public static class StreamErrors
{
    public static readonly Error InvalidLink =
        Error.Forbidden("Stream.InvalidLink", "Dinleme bağlantısı geçersiz veya süresi dolmuş.");

    public static readonly Error AudioFileMissing =
        Error.NotFound("Stream.AudioFileMissing", "Şarkının ses dosyası bulunamadı.");
}