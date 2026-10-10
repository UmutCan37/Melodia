using Melodia.Domain.Listenings;

namespace Melodia.Persistence.Repositories;

internal sealed class ListeningRecordRepository(AppDbContext context) : IListeningRecordRepository
{
    public void Add(ListeningRecord record) => context.ListeningRecords.Add(record);
}