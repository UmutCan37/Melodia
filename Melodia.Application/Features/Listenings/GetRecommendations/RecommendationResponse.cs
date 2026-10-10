namespace Melodia.Application.Features.Listenings.GetRecommendations;

public sealed record RecommendationResponse(
    Guid SongId,
    string Title,
    Guid ArtistId,
    string ArtistName,
    string GenreName,
    int DurationSeconds,
    string Reason);