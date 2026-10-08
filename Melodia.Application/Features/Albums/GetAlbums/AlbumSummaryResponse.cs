namespace Melodia.Application.Features.Albums.GetAlbums;

public sealed record AlbumSummaryResponse(
    Guid Id,
    string Title,
    Guid ArtistId,
    string ArtistName,
    DateOnly ReleaseDate);