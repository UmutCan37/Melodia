using Melodia.Domain.Listeners;
using Melodia.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Melodia.Persistence.Repositories;

internal sealed class ListenerRepository(AppDbContext context) : IListenerRepository
{
    public Task<Listener?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Listeners.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

    public Task<bool> IsEmailInUseAsync(Email email, CancellationToken cancellationToken = default) =>
        context.Listeners.AnyAsync(l => l.Email == email, cancellationToken);

    public void Add(Listener listener) => context.Listeners.Add(listener);
}