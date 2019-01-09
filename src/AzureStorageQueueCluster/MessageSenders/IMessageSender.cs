using Microsoft.WindowsAzure.Storage.Queue;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageQueueCluster.MessageSenders
{
    internal interface IMessageSender
    {
        Task SendAsync(IList<CloudQueue> cloudQueues, StorageQueueMessage message, CancellationToken cancelationToken = default(CancellationToken));
    }
}