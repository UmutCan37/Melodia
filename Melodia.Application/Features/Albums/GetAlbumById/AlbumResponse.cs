namespace Melodia.Application.Features.Albums.GetAlbumById;

public sealed record AlbumResponse(
    Guid Id,
    string Title,
    Guid ArtistId,
    string ArtistName,
    DateOnly ReleaseDate,
    IReadOnlyList<AlbumSongResponse> Songs);