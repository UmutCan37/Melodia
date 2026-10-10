using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Listeners.ChangePackage;

public sealed record ChangePackageCommand(string Package) : ICommand;