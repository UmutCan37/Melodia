using System.Security.Cryptography;
using System.Text;
using Melodia.Application.Abstractions.Storage;
using Microsoft.Extensions.Options;

namespace Melodia.Infrastructure.Storage;

internal sealed class StreamUrlSigner(IOptions<StorageOptions> options, TimeProvider timeProvider) : IStreamUrlSigner
{
    private readonly StorageOptions _options = options.Value;

    public string CreateSignedUrl(Guid songId)
    {
        var expiresAtUnix = timeProvider.GetUtcNow()
            .AddMinutes(_options.StreamUrlLifetimeMinutes)
            .ToUnixTimeSeconds();

        var signature = Sign(songId, expiresAtUnix);

        return $"/api/stream/{songId}?expires={expiresAtUnix}&signature={signature}";
    }

    public bool IsValid(Guid songId, long expiresAtUnix, string signature)
    {
        
        if (timeProvider.GetUtcNow().ToUnixTimeSeconds() > expiresAtUnix)
            return false;

        if (string.IsNullOrWhiteSpace(signature))
            return false;

        
        var expected = Encoding.ASCII.GetBytes(Sign(songId, expiresAtUnix));
        var actual = Encoding.ASCII.GetBytes(signature.ToUpperInvariant());

        return CryptographicOperations.FixedTimeEquals(expected, actual);
    }

    private string Sign(Guid songId, long expiresAtUnix)
    {
        var payload = Encoding.UTF8.GetBytes($"{songId:N}:{expiresAtUnix}");
        var key = Encoding.UTF8.GetBytes(_options.StreamSigningKey);

        return Convert.ToHexString(HMACSHA256.HashData(key, payload));
    }
}