using Melodia.Domain.Abstractions;

namespace Melodia.Domain.Playlists;

public static class PlaylistErrors
{
    public static Error NotFound(Guid playlistId) =>
        Error.NotFound("Playlist.NotFound", $"'{playlistId}' Id'li çalma listesi bulunamadı.");

    public static readonly Error NameEmpty =
        Error.Validation("Playlist.NameEmpty", "Çalma listesi adı boş olamaz.");

    public static Error NameTooLong(int maxLength) =>
        Error.Validation("Playlist.NameTooLong", $"Çalma listesi adı en fazla {maxLength} karakter olabilir.");

    public static readonly Error NotOwner =
        Error.Forbidden("Playlist.NotOwner", "Bu çalma listesi size ait değil.");

    public static readonly Error SongAlreadyInPlaylist =
        Error.Conflict("Playlist.SongAlreadyInPlaylist", "Bu şarkı zaten listede.");

    public static readonly Error SongNotInPlaylist =
        Error.NotFound("Playlist.SongNotInPlaylist", "Bu şarkı listede bulunmuyor.");
}