using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Abstractions.Data;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Listeners;
using Microsoft.EntityFrameworkCore;

namespace Melodia.Application.Features.Listenings.GetRecommendations;

internal sealed class GetRecommendationsQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    TimeProvider timeProvider) : IQueryHandler<GetRecommendationsQuery, IReadOnlyList<RecommendationResponse>>
{
    private const int LookbackDays = 30;
    private const int TopGenreCount = 3;
    private const int TopArtistCount = 5;
    private const int CandidatePoolSize = 200;

    public async Task<Result<IReadOnlyList<RecommendationResponse>>> Handle(
        GetRecommendationsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = userContext.UserId;
        var since = timeProvider.GetUtcNow().UtcDateTime.AddDays(-LookbackDays);

        var packageLevel = await context.Listeners
            .AsNoTracking()
            .Where(l => l.Id == userId)
            .Select(l => (int?)l.Package.Level)
            .FirstOrDefaultAsync(cancellationToken);

        if (packageLevel is null)
            return ListenerErrors.NotFound(userId);

        var level = packageLevel.Value;

        // 1. Zevki çıkar: son 30 günde en çok dinlenen türler ve sanatçılar
        var recentSongs =
            from r in context.ListeningRecords
            where r.ListenerId == userId && r.ListenedAtUtc >= since
            join s in context.Songs on r.SongId equals s.Id
            select s;

        var topGenres = await recentSongs
            .GroupBy(s => s.GenreId)
            .Select(g => new { GenreId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(TopGenreCount)
            .Select(x => x.GenreId)
            .ToListAsync(cancellationToken);

        var topArtists = await recentSongs
            .GroupBy(s => s.ArtistId)
            .Select(g => new { ArtistId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(TopArtistCount)
            .Select(x => x.ArtistId)
            .ToListAsync(cancellationToken);

        var hasTaste = topGenres.Count > 0;

        // 2. Adaylar: çalabildiği, daha önce hiç dinlemediği şarkılar
        var listenedSongIds = context.ListeningRecords
            .Where(r => r.ListenerId == userId)
            .Select(r => r.SongId);

        var candidates = context.Songs
            .Where(s => s.RequiredPackage.Level <= level)
            .Where(s => !listenedSongIds.Contains(s.Id));

        // Zevk varsa sadece ona uyanlar; yoksa (yeni kullanıcı) hepsi
        if (hasTaste)
            candidates = candidates.Where(s => topGenres.Contains(s.GenreId) || topArtists.Contains(s.ArtistId));

        var pool = await (
            from s in candidates
            join a in context.Artists on s.ArtistId equals a.Id
            join g in context.Genres on s.GenreId equals g.Id
            let popularity = context.ListeningRecords.Count(r => r.SongId == s.Id && r.ListenedAtUtc >= since)
            orderby popularity descending, s.Id
            select new
            {
                s.Id,
                s.Title,
                s.ArtistId,
                ArtistName = a.Name,
                s.GenreId,
                GenreName = g.Name,
                DurationSeconds = s.Duration.TotalSeconds,
                Popularity = popularity
            })
            .Take(CandidatePoolSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // 3. Puanla ve sırala: sevdiği sanatçı > sevdiği tür > popülerlik
        var recommendations = pool
            .Select(c => new
            {
                Candidate = c,
                IsTopArtist = topArtists.Contains(c.ArtistId),
                IsTopGenre = topGenres.Contains(c.GenreId)
            })
            .OrderByDescending(x => (x.IsTopArtist ? 2 : 0) + (x.IsTopGenre ? 1 : 0))
            .ThenByDescending(x => x.Candidate.Popularity)
            .Take(request.Limit)
            .Select(x => new RecommendationResponse(
                x.Candidate.Id,
                x.Candidate.Title,
                x.Candidate.ArtistId,
                x.Candidate.ArtistName,
                x.Candidate.GenreName,
                x.Candidate.DurationSeconds,
                x.IsTopArtist ? $"{x.Candidate.ArtistName} dinlediğin için"
                    : x.IsTopGenre ? $"{x.Candidate.GenreName} sevdiğin için"
                    : "Şu an popüler"))
            .ToList();

        return recommendations;
    }
}