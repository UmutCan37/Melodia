using Melodia.Domain.Abstractions;

namespace Melodia.Domain.Songs;

public sealed record AudioFile
{
    private const string AllowedExtension = ".mp3";

    private AudioFile(string filePath)
    {
        FilePath = filePath;
    }

    public string FilePath { get; }

    public static Result<AudioFile> Create(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Error.Validation("AudioFile.Empty", "Ses dosyası yolu boş olamaz.");

        filePath = filePath.Trim();

        // Kök dizinden başlayan ya da üst klasöre çıkan yollar kabul edilmez
        if (Path.IsPathRooted(filePath) || filePath.Contains(".."))
            return Error.Validation("AudioFile.InvalidPath", "Ses dosyası yolu geçersiz.");

        if (!Path.GetExtension(filePath).Equals(AllowedExtension, StringComparison.OrdinalIgnoreCase))
            return Error.Validation("AudioFile.InvalidExtension", "Sadece MP3 dosyaları desteklenir.");

        return new AudioFile(filePath);
    }
}