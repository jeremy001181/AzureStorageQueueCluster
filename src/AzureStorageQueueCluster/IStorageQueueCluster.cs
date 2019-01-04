using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageQueueCluster
{
    public interface IStorageQueueCluster
    {
        Task AddMessageAsync(StorageQueueMessage message, CancellationToken cancelationToken = default(CancellationToken));
    }
}