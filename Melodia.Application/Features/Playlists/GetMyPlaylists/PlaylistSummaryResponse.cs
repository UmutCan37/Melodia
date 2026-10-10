namespace Melodia.Application.Features.Playlists.GetMyPlaylists;

public sealed record PlaylistSummaryResponse(Guid Id, string Name, int SongCount);