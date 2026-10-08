using FluentValidation;
using Melodia.Domain.Shared;
using Melodia.Domain.Songs;

namespace Melodia.Application.Features.Songs.CreateSong;

internal sealed class CreateSongCommandValidator : AbstractValidator<CreateSongCommand>
{
    public CreateSongCommandValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("Şarkı adı boş olamaz.")
            .MaximumLength(Song.TitleMaxLength).WithMessage($"Şarkı adı en fazla {Song.TitleMaxLength} karakter olabilir.");

        RuleFor(c => c.DurationSeconds)
            .GreaterThan(0).WithMessage("Süre sıfırdan büyük olmalıdır.");

        RuleFor(c => c.ArtistId)
            .NotEmpty().WithMessage("Sanatçı Id'si boş olamaz.");

        RuleFor(c => c.GenreId)
            .NotEmpty().WithMessage("Tür Id'si boş olamaz.");

        RuleFor(c => c.RequiredPackage)
            .NotEmpty().WithMessage("Gerekli paket boş olamaz.")
            .Must(p => Package.FromName(p).IsSuccess).WithMessage("Geçersiz paket. (Basic, Gold, Premium, Elit)");

        RuleFor(c => c.AudioContent)
            .NotNull().WithMessage("Ses dosyası zorunludur.");

        RuleFor(c => c.AudioFileName)
            .NotEmpty().WithMessage("Ses dosyası adı boş olamaz.")
            .Must(n => Path.GetExtension(n).Equals(".mp3", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Sadece MP3 dosyaları desteklenir.");
    }
}