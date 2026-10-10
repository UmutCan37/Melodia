using Melodia.Domain.Artists;
using Microsoft.EntityFrameworkCore;

namespace Melodia.Persistence.Repositories;

internal sealed class ArtistRepository(AppDbContext context) : IArtistRepository
{
    public Task<Artist?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Artists.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Artists.AnyAsync(a => a.Id == id, cancellationToken);

    public void Add(Artist artist) => context.Artists.Add(artist);

    public void Remove(Artist artist) => context.Artists.Remove(artist);
}