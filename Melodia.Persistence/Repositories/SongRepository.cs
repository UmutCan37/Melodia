using Melodia.Domain.Songs;
using Microsoft.EntityFrameworkCore;

namespace Melodia.Persistence.Repositories;

internal sealed class SongRepository(AppDbContext context) : ISongRepository
{
    public Task<Song?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Songs.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    // Yayından kaldırılmış şarkılar da türe bağlı; silmeyi engellemeleri gerekiyor
    public Task<bool> ExistsByGenreAsync(Guid genreId, CancellationToken cancellationToken = default) =>
        context.Songs.IgnoreQueryFilters().AnyAsync(s => s.GenreId == genreId, cancellationToken);

    // Aynı sebeple sanatçıda da silinmişler sayılıyor
    public Task<bool> ExistsByArtistAsync(Guid artistId, CancellationToken cancellationToken = default) =>
        context.Songs.IgnoreQueryFilters().AnyAsync(s => s.ArtistId == artistId, cancellationToken);

    // Albümde silinmişler sayılmıyor: albüm silinince onların AlbumId'si SetNull ile boşalıyor
    public Task<bool> ExistsByAlbumAsync(Guid albumId, CancellationToken cancellationToken = default) =>
        context.Songs.AnyAsync(s => s.AlbumId == albumId, cancellationToken);

    public void Add(Song song) => context.Songs.Add(song);

    public void Remove(Song song) => context.Songs.Remove(song);
}