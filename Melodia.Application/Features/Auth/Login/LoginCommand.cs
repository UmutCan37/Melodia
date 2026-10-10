using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<LoginResponse>;