using Melodia.Application.Abstractions.Data;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Application.Common;
using Melodia.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Melodia.Application.Features.Artists.GetArtists;

internal sealed class GetArtistsQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetArtistsQuery, PagedResponse<ArtistSummaryResponse>>
{
    public async Task<Result<PagedResponse<ArtistSummaryResponse>>> Handle(
        GetArtistsQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Artists.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(a => a.Name.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(a => a.Name)
            .ThenBy(a => a.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new ArtistSummaryResponse(a.Id, a.Name))
            .ToListAsync(cancellationToken);

        return new PagedResponse<ArtistSummaryResponse>(items, request.Page, request.PageSize, totalCount);
    }
}