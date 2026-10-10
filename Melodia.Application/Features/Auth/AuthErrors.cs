using Melodia.Domain.Abstractions;

namespace Melodia.Application.Features.Auth;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials =
        Error.Unauthorized("Auth.InvalidCredentials", "E-posta veya şifre hatalı.");

    public static readonly Error InvalidRefreshToken =
        Error.Unauthorized("Auth.InvalidRefreshToken", "Oturum süresi dolmuş veya geçersiz. Lütfen tekrar giriş yapın.");
}