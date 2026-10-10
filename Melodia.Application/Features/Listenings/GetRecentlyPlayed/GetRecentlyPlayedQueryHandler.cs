using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Abstractions.Data;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Melodia.Application.Features.Listenings.GetRecentlyPlayed;

internal sealed class GetRecentlyPlayedQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext) : IQueryHandler<GetRecentlyPlayedQuery, IReadOnlyList<RecentlyPlayedResponse>>
{
    public async Task<Result<IReadOnlyList<RecentlyPlayedResponse>>> Handle(
        GetRecentlyPlayedQuery request,
        CancellationToken cancellationToken)
    {
        var userId = userContext.UserId;

        // 1. Her şarkı bir kez, en son dinlendiği zamanla
        var lastListens = context.ListeningRecords
            .Where(r => r.ListenerId == userId)
            .GroupBy(r => r.SongId)
            .Select(g => new
            {
                SongId = g.Key,
                LastListenedAtUtc = g.Max(r => r.ListenedAtUtc)
            });

        // 2. Şarkı ve sanatçı bilgisiyle birleştir, en yeniden eskiye
        var items = await (
            from l in lastListens
            join song in context.Songs on l.SongId equals song.Id
            join artist in context.Artists on song.ArtistId equals artist.Id
            orderby l.LastListenedAtUtc descending
            select new RecentlyPlayedResponse(
                song.Id,
                song.Title,
                artist.Id,
                artist.Name,
                song.Duration.TotalSeconds,
                l.LastListenedAtUtc))
            .Take(request.Limit)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return items;
    }
}