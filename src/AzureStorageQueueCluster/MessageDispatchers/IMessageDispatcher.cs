using Microsoft.WindowsAzure.Storage.Queue;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageQueueCluster.MessageDispatchers
{
    internal interface IMessageDispatcher
    {
        Task SendAsync(IList<CloudQueue> cloudQueues, StorageQueueMessage message, CancellationToken cancelationToken = default(CancellationToken));
    }
}