using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Abstractions.Data;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Melodia.Application.Features.Playlists.GetMyPlaylists;

internal sealed class GetMyPlaylistsQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext) : IQueryHandler<GetMyPlaylistsQuery, IReadOnlyList<PlaylistSummaryResponse>>
{
    public async Task<Result<IReadOnlyList<PlaylistSummaryResponse>>> Handle(
        GetMyPlaylistsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = userContext.UserId;

        var playlists = await context.Playlists
            .AsNoTracking()
            .Where(p => p.OwnerId == userId)
            .OrderBy(p => p.Name)
            .Select(p => new PlaylistSummaryResponse(p.Id, p.Name, p.Songs.Count))
            .ToListAsync(cancellationToken);

        return playlists;
    }
}