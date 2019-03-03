using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageQueueCluster.MessageDispatchers
{
    internal class ActivePassiveMessageDispatcher : IMessageDispatcher
    {
        private IReadOnlyList<CloudQueue> cloudQueues;

        public ActivePassiveMessageDispatcher(IReadOnlyList<CloudQueue> cloudQueues)
        {
            this.cloudQueues = cloudQueues;
        }

        public Task SendAsync(StorageQueueMessage message, CancellationToken cancelationToken = default(CancellationToken))
        {
            return AddMessageRecusivelyAsync(cloudQueues, message, 0, 0, cancelationToken);
        }

        private Task AddMessageRecusivelyAsync(IReadOnlyList<CloudQueue> cloudQueues, StorageQueueMessage message, int index, int failed, CancellationToken cancelationToken)
        {
            if (index >= cloudQueues.Count)
            {
                return Task.CompletedTask;
            }
            var next = index + 1;
            var cloudQueue = cloudQueues[index];
            try
            {
                return cloudQueue.AddMessageAsync(message.Data, message.TimeToLive, message.InitialVisibilityDelay, message.Options, message.OperationContext, cancelationToken);
            }
            catch (Exception ex) when (ex.IsTransilient())
            {
                if (++failed == cloudQueues.Count)
                {
                    throw;
                }

                return AddMessageRecusivelyAsync(cloudQueues, message, next, failed, cancelationToken);
            }
        }
    }
}