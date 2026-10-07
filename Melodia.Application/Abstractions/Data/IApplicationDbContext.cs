using Melodia.Domain.Albums;
using Melodia.Domain.Artists;
using Melodia.Domain.Genres;
using Melodia.Domain.Listeners;
using Melodia.Domain.Listenings;
using Melodia.Domain.Playlists;
using Melodia.Domain.Songs;
using Microsoft.EntityFrameworkCore;

namespace Melodia.Application.Abstractions.Data;

// Sadece query handler'larda kullanılır; yazma işlemleri repository + IUnitOfWork üzerinden yapılır
public interface IApplicationDbContext
{
    DbSet<Song> Songs { get; }
    DbSet<Artist> Artists { get; }
    DbSet<Album> Albums { get; }
    DbSet<Genre> Genres { get; }
    DbSet<Listener> Listeners { get; }
    DbSet<Playlist> Playlists { get; }
    DbSet<ListeningRecord> ListeningRecords { get; }
}