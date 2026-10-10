using Melodia.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Melodia.Infrastructure.Authentication;

internal sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            return Guid.TryParse(value, out var userId)
                ? userId
                : throw new InvalidOperationException("Kimliği doğrulanmış kullanıcı bulunamadı.");
        }
    }
}