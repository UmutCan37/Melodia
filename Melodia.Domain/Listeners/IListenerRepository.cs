using Melodia.Domain.Shared;

namespace Melodia.Domain.Listeners;

public interface IListenerRepository
{
    Task<Listener?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> IsEmailInUseAsync(Email email, CancellationToken cancellationToken = default);
    void Add(Listener listener);
}