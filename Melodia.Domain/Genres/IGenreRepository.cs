namespace Melodia.Domain.Genres;

public interface IGenreRepository
{
    Task<Genre?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> IsNameInUseAsync(string name, CancellationToken cancellationToken = default);
    void Add(Genre genre);
    void Remove(Genre genre);
}