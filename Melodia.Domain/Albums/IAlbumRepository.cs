namespace Melodia.Domain.Albums;

public interface IAlbumRepository
{
    Task<Album?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Album album);
    void Remove(Album album);
}