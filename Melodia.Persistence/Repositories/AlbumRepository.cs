using Melodia.Domain.Albums;
using Microsoft.EntityFrameworkCore;

namespace Melodia.Persistence.Repositories;

internal sealed class AlbumRepository(AppDbContext context) : IAlbumRepository
{
    public Task<Album?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Albums.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<bool> ExistsByArtistAsync(Guid artistId, CancellationToken cancellationToken = default) =>
        context.Albums.AnyAsync(a => a.ArtistId == artistId, cancellationToken);

    public void Add(Album album) => context.Albums.Add(album);

    public void Remove(Album album) => context.Albums.Remove(album);
}