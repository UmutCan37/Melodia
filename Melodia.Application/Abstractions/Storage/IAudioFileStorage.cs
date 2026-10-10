namespace Melodia.Application.Abstractions.Storage;

public interface IAudioFileStorage
{
    Task<string> SaveAsync(Stream content, string fileName, CancellationToken cancellationToken = default);

    Task DeleteAsync(string filePath, CancellationToken cancellationToken = default);

    Stream? OpenRead(string filePath);

}