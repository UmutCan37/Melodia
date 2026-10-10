using MediatR;
using Melodia.Application.Abstractions.Data;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Albums;
using Melodia.Domain.Artists;
using Melodia.Domain.Genres;
using Melodia.Domain.Listeners;
using Melodia.Domain.Listenings;
using Melodia.Domain.Playlists;
using Melodia.Domain.Songs;
using Melodia.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Melodia.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options, IPublisher publisher)
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options),
      IApplicationDbContext,
      IUnitOfWork,
      IDatabaseTransactionManager
{
    public DbSet<Song> Songs => Set<Song>();
    public DbSet<Artist> Artists => Set<Artist>();
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Listener> Listeners => Set<Listener>();
    public DbSet<Playlist> Playlists => Set<Playlist>();
    public DbSet<ListeningRecord> ListeningRecords => Set<ListeningRecord>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. Kaydetmeden önce event'leri topla ve entity'lerden temizle
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var events = entity.DomainEvents.ToList();
                entity.ClearDomainEvents();
                return events;
            })
            .ToList();

        // 2. Kaydet
        var result = await base.SaveChangesAsync(cancellationToken);

        // 3. Kayıt başarılıysa event'leri dağıt
        foreach (var domainEvent in domainEvents)
            await publisher.Publish(domainEvent, cancellationToken);

        return result;
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        Database.BeginTransactionAsync(cancellationToken);
}