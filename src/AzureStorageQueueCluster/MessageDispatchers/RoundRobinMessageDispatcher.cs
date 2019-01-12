using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageQueueCluster.MessageDispatchers
{
    internal class RoundRobinMessageDispatcher : IMessageDispatcher
    {
        public Task SendAsync(IList<CloudQueue> cloudQueues, StorageQueueMessage message, CancellationToken cancelationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}