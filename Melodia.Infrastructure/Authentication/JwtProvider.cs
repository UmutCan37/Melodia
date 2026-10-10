using System.Security.Claims;
using System.Text;
using Melodia.Application.Abstractions.Authentication;
using Melodia.Domain.Shared;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Melodia.Infrastructure.Authentication;

internal sealed class JwtProvider(IOptions<JwtOptions> options, TimeProvider timeProvider) : IJwtProvider
{
    public const string PackageClaim = "package";
    public const string RoleClaim = "role";

    private readonly JwtOptions _options = options.Value;

    public string CreateAccessToken(Guid userId, string email, IEnumerable<string> roles, Package package)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(PackageClaim, package.Name)
        };

        claims.AddRange(roles.Select(role => new Claim(RoleClaim, role)));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var now = timeProvider.GetUtcNow().UtcDateTime;

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            IssuedAt = now,
            NotBefore = now,
            Expires = now.AddMinutes(_options.AccessTokenMinutes),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}