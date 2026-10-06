using Melodia.Domain.Abstractions;

namespace Melodia.Domain.Songs;

public static class SongErrors
{
    public static Error NotFound(Guid songId) =>
        Error.NotFound("Song.NotFound", $"'{songId}' Id'li şarkı bulunamadı.");

    public static readonly Error TitleEmpty =
        Error.Validation("Song.TitleEmpty", "Şarkı adı boş olamaz.");

    public static Error TitleTooLong(int maxLength) =>
        Error.Validation("Song.TitleTooLong", $"Şarkı adı en fazla {maxLength} karakter olabilir.");

    public static readonly Error PackageNotSufficient =
        Error.Forbidden("Song.PackageNotSufficient", "Bu şarkıyı dinlemek için paketiniz yeterli değil.");
}