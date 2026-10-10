using FluentValidation;
using Melodia.Domain.Playlists;

namespace Melodia.Application.Features.Playlists.CreatePlaylist;

internal sealed class CreatePlaylistCommandValidator : AbstractValidator<CreatePlaylistCommand>
{
    public CreatePlaylistCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Çalma listesi adı boş olamaz.")
            .MaximumLength(Playlist.NameMaxLength).WithMessage($"Çalma listesi adı en fazla {Playlist.NameMaxLength} karakter olabilir.");
    }
}