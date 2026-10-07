using Melodia.Domain.Abstractions;

namespace Melodia.Domain.Genres;

public static class GenreErrors
{
    public static Error NotFound(Guid genreId) =>
        Error.NotFound("Genre.NotFound", $"'{genreId}' Id'li tür bulunamadı.");

    public static readonly Error NameEmpty =
        Error.Validation("Genre.NameEmpty", "Tür adı boş olamaz.");

    public static Error NameTooLong(int maxLength) =>
        Error.Validation("Genre.NameTooLong", $"Tür adı en fazla {maxLength} karakter olabilir.");

    public static readonly Error NameAlreadyExists =
        Error.Conflict("Genre.NameAlreadyExists", "Bu isimde bir tür zaten var.");

    public static readonly Error HasSongs =
       Error.Conflict("Genre.HasSongs", "Bu türe ait şarkılar var; önce şarkıları başka bir türe taşıyın.");
}