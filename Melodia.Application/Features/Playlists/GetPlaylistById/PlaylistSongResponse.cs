namespace Melodia.Application.Features.Playlists.GetPlaylistById;

public sealed record PlaylistSongResponse(
    Guid SongId,
    int Position,
    string Title,
    string ArtistName,
    int DurationSeconds,
    string RequiredPackage,
    DateTime AddedAtUtc,
    bool IsAvailable);