using Melodia.Domain.Shared;

namespace Melodia.Application.Abstractions.Authentication;

public interface IJwtProvider
{
    string CreateAccessToken(Guid userId, string email, IEnumerable<string> roles, Package package);
}