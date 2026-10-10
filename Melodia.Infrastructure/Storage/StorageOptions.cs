namespace Melodia.Infrastructure.Storage;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    public string AudioRootPath { get; init; } = null!;
    public string StreamSigningKey { get; init; } = null!;
    public int StreamUrlLifetimeMinutes { get; init; } = 5;
}