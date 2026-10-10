using Melodia.Application.Abstractions.Data;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Albums;
using Melodia.Domain.Artists;
using Melodia.Domain.Genres;
using Melodia.Domain.Listeners;
using Melodia.Domain.Listenings;
using Melodia.Domain.Playlists;
using Melodia.Domain.Songs;
using Melodia.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Melodia.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("'Database' bağlantı dizesi bulunamadı.");

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

        
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IDatabaseTransactionManager>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IArtistRepository, ArtistRepository>();
        services.AddScoped<IAlbumRepository, AlbumRepository>();
        services.AddScoped<ISongRepository, SongRepository>();
        services.AddScoped<IListenerRepository, ListenerRepository>();
        services.AddScoped<IPlaylistRepository, PlaylistRepository>();
        services.AddScoped<IListeningRecordRepository, ListeningRecordRepository>();

        return services;
    }
}