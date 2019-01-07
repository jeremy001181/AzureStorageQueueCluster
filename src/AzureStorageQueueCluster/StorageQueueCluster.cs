using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureStorageQueueCluster
{
    internal class StorageQueueCluster : IStorageQueueCluster
    {
        private readonly IList<CloudQueue> cloudQueues;

        internal StorageQueueCluster(IList<CloudQueue> cloudQueues)
        {
            if (cloudQueues == null || cloudQueues.Count == 0) {
                throw new ArgumentException("Cannot initialize cluster with no queue", nameof(cloudQueues));
            }

            this.cloudQueues = cloudQueues;
        }
        
        public async Task AddMessageAsync(StorageQueueMessage message, CancellationToken cancelationToken = default(CancellationToken)) {
            await AddMessageRecusivelyAsync(0, 0, message, cancelationToken);
        }

        private async Task AddMessageRecusivelyAsync(int index, int failed, StorageQueueMessage message, CancellationToken cancelationToken)
        {
            if (index >= cloudQueues.Count)
            {
                return;
            }
            var next = index + 1;
            var cloudQueue = cloudQueues[index];
            try
            {
                await cloudQueue.AddMessageAsync(message.Data, message.TimeToLive, message.InitialVisibilityDelay, message.Options, message.OperationContext, cancelationToken)
                                .ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.IsTransilient())
            {
                if (++failed == cloudQueues.Count)
                {
                    throw;
                }

                await AddMessageRecusivelyAsync(next, failed, message, cancelationToken);
            }
        }
    }
}
