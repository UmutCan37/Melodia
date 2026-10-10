namespace Melodia.Application.Features.Songs.GetSongs;

public sealed record SongSummaryResponse(
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