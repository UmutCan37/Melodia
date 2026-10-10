using Melodia.Domain.Albums;
using Melodia.Domain.Artists;
using Melodia.Domain.Genres;
using Melodia.Domain.Songs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Melodia.Persistence.Configurations;

internal sealed class SongConfiguration : IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> builder)
    {
        builder.ToTable("Songs");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.Title)
            .HasMaxLength(Song.TitleMaxLength)
            .IsRequired();

        // Value object'ler: ayrı tablo değil, Songs tablosunda kolon
        builder.ComplexProperty(s => s.Duration, duration =>
        {
            duration.Property(d => d.TotalSeconds).HasColumnName("DurationSeconds");
        });

        builder.ComplexProperty(s => s.AudioFile, audio =>
        {
            audio.Property(a => a.FilePath).HasColumnName("AudioFilePath").HasMaxLength(500);
        });

        builder.ComplexProperty(s => s.RequiredPackage, package =>
        {
            package.Property(p => p.Name).HasColumnName("RequiredPackage").HasMaxLength(20);
            package.Property(p => p.Level).HasColumnName("RequiredPackageLevel");
        });

        // İlişkiler (navigation property yok)
        builder.HasOne<Artist>().WithMany().HasForeignKey(s => s.ArtistId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Genre>().WithMany().HasForeignKey(s => s.GenreId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Album>().WithMany().HasForeignKey(s => s.AlbumId).OnDelete(DeleteBehavior.SetNull);

        // Soft delete: bütün sorgular silinmişleri otomatik dışarıda bırakır
        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.HasIndex(s => s.Title);

        builder.Ignore(s => s.DomainEvents);
    }
}