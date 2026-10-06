namespace Melodia.Domain.Playlists;

public interface IPlaylistRepository
{
    Task<Playlist?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Playlist playlist);
    void Remove(Playlist playlist);
}