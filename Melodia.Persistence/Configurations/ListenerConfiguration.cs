using Melodia.Domain.Listeners;
using Melodia.Domain.Shared;
using Melodia.Persistence.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Melodia.Persistence.Configurations;

internal sealed class ListenerConfiguration : IEntityTypeConfiguration<Listener>
{
    public void Configure(EntityTypeBuilder<Listener> builder)
    {
        builder.ToTable("Listeners");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedNever();

        // Email: tek kolon, unique index lazım → value converter
        builder.Property(l => l.Email)
            .HasConversion(email => email.Value, value => Email.Create(value).Value)
            .HasColumnName("Email")
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(l => l.Email).IsUnique();

        builder.Property(l => l.DisplayName)
            .HasMaxLength(Listener.DisplayNameMaxLength)
            .IsRequired();

        builder.ComplexProperty(l => l.Package, package =>
        {
            package.Property(p => p.Name).HasColumnName("Package").HasMaxLength(20);
            package.Property(p => p.Level).HasColumnName("PackageLevel");
        });

        builder.Property(l => l.RegisteredAtUtc).IsRequired();

        // Listener, Identity'deki kullanıcıyla aynı Id'yi paylaşır
        builder.HasOne<AppUser>()
            .WithOne()
            .HasForeignKey<Listener>(l => l.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(l => l.DomainEvents);
    }
}