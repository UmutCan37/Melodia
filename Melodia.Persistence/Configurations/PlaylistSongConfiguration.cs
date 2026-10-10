using Melodia.Domain.Playlists;
using Melodia.Domain.Songs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Melodia.Persistence.Configurations;

internal sealed class PlaylistSongConfiguration : IEntityTypeConfiguration<PlaylistSong>
{
    public void Configure(EntityTypeBuilder<PlaylistSong> builder)
    {
        builder.ToTable("PlaylistSongs");

        builder.HasKey(ps => ps.Id);
        builder.Property(ps => ps.Id).ValueGeneratedNever();

        builder.Property(ps => ps.Position).IsRequired();
        builder.Property(ps => ps.AddedAtUtc).IsRequired();

        builder.HasOne<Song>()
            .WithMany()
            .HasForeignKey(ps => ps.SongId)
            .OnDelete(DeleteBehavior.Restrict);

        // Aynı şarkı aynı listede iki kez olamaz
        builder.HasIndex(ps => new { ps.PlaylistId, ps.SongId }).IsUnique();

        builder.Ignore(ps => ps.DomainEvents);
    }
}