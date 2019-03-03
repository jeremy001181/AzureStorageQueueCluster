using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageQueueCluster.MessageDispatchers
{
    internal interface IMessageDispatcher
    {
        Task SendAsync(StorageQueueMessage message, CancellationToken cancelationToken = default(CancellationToken));
    }
}