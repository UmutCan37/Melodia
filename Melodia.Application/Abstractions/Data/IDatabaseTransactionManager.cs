using Microsoft.EntityFrameworkCore.Storage;

namespace Melodia.Application.Abstractions.Data;

public interface IDatabaseTransactionManager
{
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}