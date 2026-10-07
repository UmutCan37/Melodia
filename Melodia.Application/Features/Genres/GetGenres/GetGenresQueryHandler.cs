using Melodia.Application.Abstractions.Data;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Melodia.Application.Features.Genres.GetGenres;

internal sealed class GetGenresQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetGenresQuery, IReadOnlyList<GenreResponse>>
{
    public async Task<Result<IReadOnlyList<GenreResponse>>> Handle(
        GetGenresQuery request,
        CancellationToken cancellationToken)
    {
        var genres = await context.Genres
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .Select(g => new GenreResponse(g.Id, g.Name))
            .ToListAsync(cancellationToken);

        return genres;
    }
}