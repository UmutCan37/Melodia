using Melodia.Domain.Artists;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Melodia.Persistence.Configurations;

internal sealed class ArtistConfiguration : IEntityTypeConfiguration<Artist>
{
    public void Configure(EntityTypeBuilder<Artist> builder)
    {
        builder.ToTable("Artists");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.Name)
            .HasMaxLength(Artist.NameMaxLength)
            .IsRequired();

        builder.Property(a => a.Biography)
            .HasMaxLength(Artist.BiographyMaxLength);

        builder.HasIndex(a => a.Name);

        builder.Ignore(a => a.DomainEvents);
    }
}