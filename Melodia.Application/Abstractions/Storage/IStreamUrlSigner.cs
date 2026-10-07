namespace Melodia.Application.Abstractions.Storage;

public interface IStreamUrlSigner
{
    string CreateSignedUrl(Guid songId);

    bool IsValid(Guid songId, long expiresAtUnix, string signature);
}