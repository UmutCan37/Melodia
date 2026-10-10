using Melodia.Domain.Albums;
using Melodia.Domain.Artists;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Melodia.Persistence.Configurations;

internal sealed class AlbumConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> builder)
    {
        builder.ToTable("Albums");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.Title)
            .HasMaxLength(Album.TitleMaxLength)
            .IsRequired();

        builder.Property(a => a.ReleaseDate).IsRequired();

        // Navigation property olmadan foreign key
        builder.HasOne<Artist>()
            .WithMany()
            .HasForeignKey(a => a.ArtistId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.Title);

        builder.Ignore(a => a.DomainEvents);
    }
}