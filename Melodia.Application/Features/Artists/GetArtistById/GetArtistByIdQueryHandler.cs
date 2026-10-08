using Melodia.Application.Abstractions.Data;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Artists;
using Microsoft.EntityFrameworkCore;

namespace Melodia.Application.Features.Artists.GetArtistById;

internal sealed class GetArtistByIdQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetArtistByIdQuery, ArtistResponse>
{
    public async Task<Result<ArtistResponse>> Handle(GetArtistByIdQuery request, CancellationToken cancellationToken)
    {
        var artist = await context.Artists
            .AsNoTracking()
            .Where(a => a.Id == request.ArtistId)
            .Select(a => new ArtistResponse(a.Id, a.Name, a.Biography))
            .FirstOrDefaultAsync(cancellationToken);

        if (artist is null)
            return ArtistErrors.NotFound(request.ArtistId);

        return artist;
    }
}