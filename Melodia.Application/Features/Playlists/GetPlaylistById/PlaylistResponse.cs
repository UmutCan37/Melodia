namespace Melodia.Application.Features.Playlists.GetPlaylistById;

public sealed record PlaylistResponse(
    Guid Id,
    string Name,
    IReadOnlyList<PlaylistSongResponse> Songs);