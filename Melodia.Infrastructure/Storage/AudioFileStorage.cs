using Melodia.Application.Abstractions.Storage;
using Microsoft.Extensions.Options;

namespace Melodia.Infrastructure.Storage;

internal sealed class AudioFileStorage(IOptions<StorageOptions> options) : IAudioFileStorage
{
    private const string AllowedExtension = ".mp3";
    private const string SongsFolder = "songs";

    private readonly string _rootPath = Path.GetFullPath(options.Value.AudioRootPath);

    public async Task<string> SaveAsync(Stream content, string fileName, CancellationToken cancellationToken = default)
    {
        if (!Path.GetExtension(fileName).Equals(AllowedExtension, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Sadece MP3 dosyaları kaydedilebilir.");

        // Kullanıcının verdiği ad diske hiç yansımaz; adı biz üretiyoruz
        var relativePath = $"{SongsFolder}/{Guid.CreateVersion7():N}{AllowedExtension}";
        var fullPath = ResolveFullPath(relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var file = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);
        await content.CopyToAsync(file, cancellationToken);

        return relativePath;
    }

    public Task DeleteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = ResolveFullPath(filePath);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }

    public Stream? OpenRead(string filePath)
    {
        var fullPath = ResolveFullPath(filePath);

        return File.Exists(fullPath)
            ? new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true)
            : null;
    }

    // Göreli yolu tam yola çevirir; kök klasörün dışına çıkmaya çalışan yolları reddeder
    private string ResolveFullPath(string relativePath)
    {
        var fullPath = Path.GetFullPath(Path.Combine(_rootPath, relativePath));

        if (!fullPath.StartsWith(_rootPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Geçersiz dosya yolu.");

        return fullPath;
    }
}