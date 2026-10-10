namespace Melodia.Application.Features.Listenings.GetRecentlyPlayed;

public sealed record RecentlyPlayedResponse(
    Guid SongId,
    string Title,
    Guid ArtistId,
    string ArtistName,
    int DurationSeconds,
    DateTime ListenedAtUtc);