namespace Melodia.Domain.Songs;

public interface ISongRepository
{
    Task<Song?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Song song);
    void Remove(Song song);

    Task<bool> ExistsByGenreAsync(Guid genreId, CancellationToken cancellationToken = default);
}