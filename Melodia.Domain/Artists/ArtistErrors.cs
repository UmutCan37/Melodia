using Melodia.Domain.Abstractions;

namespace Melodia.Domain.Artists;

public static class ArtistErrors
{
    public static Error NotFound(Guid artistId) =>
        Error.NotFound("Artist.NotFound", $"'{artistId}' Id'li sanatçı bulunamadı.");

    public static readonly Error NameEmpty =
        Error.Validation("Artist.NameEmpty", "Sanatçı adı boş olamaz.");

    public static Error NameTooLong(int maxLength) =>
        Error.Validation("Artist.NameTooLong", $"Sanatçı adı en fazla {maxLength} karakter olabilir.");

    public static Error BiographyTooLong(int maxLength) =>
        Error.Validation("Artist.BiographyTooLong", $"Biyografi en fazla {maxLength} karakter olabilir.");
}