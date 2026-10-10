using Melodia.Domain.Playlists;
using Microsoft.EntityFrameworkCore;

namespace Melodia.Persistence.Repositories;

internal sealed class PlaylistRepository(AppDbContext context) : IPlaylistRepository
{
    
    public Task<Playlist?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Playlists
            .Include(p => p.Songs)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public void Add(Playlist playlist) => context.Playlists.Add(playlist);

    public void Remove(Playlist playlist) => context.Playlists.Remove(playlist);
}