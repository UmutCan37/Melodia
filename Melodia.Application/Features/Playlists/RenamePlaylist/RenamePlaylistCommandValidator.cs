using FluentValidation;
using Melodia.Domain.Playlists;

namespace Melodia.Application.Features.Playlists.RenamePlaylist;

internal sealed class RenamePlaylistCommandValidator : AbstractValidator<RenamePlaylistCommand>
{
    public RenamePlaylistCommandValidator()
    {
        RuleFor(c => c.PlaylistId)
            .NotEmpty().WithMessage("Çalma listesi Id'si boş olamaz.");

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Çalma listesi adı boş olamaz.")
            .MaximumLength(Playlist.NameMaxLength).WithMessage($"Çalma listesi adı en fazla {Playlist.NameMaxLength} karakter olabilir.");
    }
}