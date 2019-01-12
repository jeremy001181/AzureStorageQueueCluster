using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureStorageQueueCluster.MessageDispatchers;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureStorageQueueCluster
{
    internal class StorageQueueCluster : IStorageQueueCluster
    {
        private readonly IList<CloudQueue> cloudQueues;
        private readonly IMessageDispatcher sender;

        internal StorageQueueCluster(IList<CloudQueue> cloudQueues, IMessageDispatcher sender)
        {
            if (cloudQueues == null || cloudQueues.Count == 0) {
                throw new ArgumentException("Cannot initialize cluster with no queue", nameof(cloudQueues));
            }

            this.cloudQueues = cloudQueues;
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }
        
        public async Task AddMessageAsync(StorageQueueMessage message, CancellationToken cancelationToken = default(CancellationToken)) {

            await sender.SendAsync(cloudQueues, message, cancelationToken);

        }        
    }
}
