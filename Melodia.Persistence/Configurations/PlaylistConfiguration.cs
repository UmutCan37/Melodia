using Melodia.Domain.Listeners;
using Melodia.Domain.Playlists;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Melodia.Persistence.Configurations;

internal sealed class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
{
    public void Configure(EntityTypeBuilder<Playlist> builder)
    {
        builder.ToTable("Playlists");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Name)
            .HasMaxLength(Playlist.NameMaxLength)
            .IsRequired();

        builder.HasOne<Listener>()
            .WithMany()
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Aggregate'in parçası: liste silinince şarkı kayıtları da gider
        builder.HasMany(p => p.Songs)
            .WithOne()
            .HasForeignKey(ps => ps.PlaylistId)
            .OnDelete(DeleteBehavior.Cascade);

        // EF, dışarıya açık IReadOnlyCollection'a değil, içerideki _songs listesine yazsın
        builder.Navigation(p => p.Songs).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(p => p.DomainEvents);
    }
}