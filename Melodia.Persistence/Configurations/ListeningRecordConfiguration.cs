using Melodia.Domain.Listeners;
using Melodia.Domain.Listenings;
using Melodia.Domain.Songs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Melodia.Persistence.Configurations;

internal sealed class ListeningRecordConfiguration : IEntityTypeConfiguration<ListeningRecord>
{
    public void Configure(EntityTypeBuilder<ListeningRecord> builder)
    {
        builder.ToTable("ListeningRecords");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.Property(r => r.ListenedAtUtc).IsRequired();

        builder.HasOne<Listener>()
            .WithMany()
            .HasForeignKey(r => r.ListenerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Song>()
            .WithMany()
            .HasForeignKey(r => r.SongId)
            .OnDelete(DeleteBehavior.Restrict);

        // "Son dinlediklerim" ve öneriler: bir kullanıcının belirli tarihten sonraki dinlemeleri
        builder.HasIndex(r => new { r.ListenerId, r.ListenedAtUtc });

        // Öneriler: bir şarkının belirli tarihten sonraki dinlenme sayısı (popülerlik)
        builder.HasIndex(r => new { r.SongId, r.ListenedAtUtc });

        builder.Ignore(r => r.DomainEvents);
    }
}