namespace Melodia.Application.Features.Songs.GetSongById;

public sealed record SongResponse(
    Guid Id,
    string Title,
    int DurationSeconds,
    Guid ArtistId,
    string ArtistName,
    Guid GenreId,
    string GenreName,
    Guid? AlbumId,
    string? AlbumTitle,
    string RequiredPackage);