using Melodia.Domain.Abstractions;

namespace Melodia.Domain.Albums;

public static class AlbumErrors
{
    public static Error NotFound(Guid albumId) =>
        Error.NotFound("Album.NotFound", $"'{albumId}' Id'li albüm bulunamadı.");

    public static readonly Error TitleEmpty =
        Error.Validation("Album.TitleEmpty", "Albüm adı boş olamaz.");

    public static Error TitleTooLong(int maxLength) =>
        Error.Validation("Album.TitleTooLong", $"Albüm adı en fazla {maxLength} karakter olabilir.");

    public static readonly Error ArtistMismatch =
        Error.Conflict("Album.ArtistMismatch", "Şarkı, sanatçısına ait olmayan bir albüme eklenemez.");

    public static readonly Error HasSongs =
        Error.Conflict("Album.HasSongs", "Bu albümde şarkılar var; önce şarkıları albümden çıkarın.");
}