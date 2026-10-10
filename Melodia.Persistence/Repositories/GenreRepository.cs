using Melodia.Domain.Genres;
using Microsoft.EntityFrameworkCore;

namespace Melodia.Persistence.Repositories;

internal sealed class GenreRepository(AppDbContext context) : IGenreRepository
{
    public Task<Genre?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Genres.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Genres.AnyAsync(g => g.Id == id, cancellationToken);

    public Task<bool> IsNameInUseAsync(string name, CancellationToken cancellationToken = default) =>
        context.Genres.AnyAsync(g => g.Name == name, cancellationToken);

    public void Add(Genre genre) => context.Genres.Add(genre);

    public void Remove(Genre genre) => context.Genres.Remove(genre);
}