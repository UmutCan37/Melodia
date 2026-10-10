using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Auth.Register;

public sealed record RegisterCommand(string Email, string Password, string DisplayName) : ICommand<Guid>;