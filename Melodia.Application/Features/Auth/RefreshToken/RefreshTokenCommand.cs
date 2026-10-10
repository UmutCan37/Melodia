using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Auth.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<RefreshTokenResponse>;