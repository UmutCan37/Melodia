namespace Melodia.Domain.Artists;

public interface IArtistRepository
{
    Task<Artist?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Artist artist);
    void Remove(Artist artist);
}