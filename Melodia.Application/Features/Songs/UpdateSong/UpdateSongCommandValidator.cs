using FluentValidation;
using Melodia.Domain.Shared;
using Melodia.Domain.Songs;

namespace Melodia.Application.Features.Songs.UpdateSong;

internal sealed class UpdateSongCommandValidator : AbstractValidator<UpdateSongCommand>
{
    public UpdateSongCommandValidator()
    {
        RuleFor(c => c.SongId)
            .NotEmpty().WithMessage("Şarkı Id'si boş olamaz.");

        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("Şarkı adı boş olamaz.")
            .MaximumLength(Song.TitleMaxLength).WithMessage($"Şarkı adı en fazla {Song.TitleMaxLength} karakter olabilir.");

        RuleFor(c => c.RequiredPackage)
            .NotEmpty().WithMessage("Gerekli paket boş olamaz.")
            .Must(p => Package.FromName(p).IsSuccess).WithMessage("Geçersiz paket. (Basic, Gold, Premium, Elit)");
    }
}