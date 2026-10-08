namespace Melodia.Application.Features.Albums.GetAlbumById;

public sealed record AlbumSongResponse(
    Guid Id,
    string Title,
    int DurationSeconds,
    string RequiredPackage);