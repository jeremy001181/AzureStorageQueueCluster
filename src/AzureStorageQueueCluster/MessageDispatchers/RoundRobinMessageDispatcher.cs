using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageQueueCluster.MessageDispatchers
{
    internal class RoundRobinMessageDispatcher : IMessageDispatcher
    {
        private IList<CloudQueue> cloudQueues;
        private RoundRobbinNumberResolver roundRobbinNumberResolver;

        public RoundRobinMessageDispatcher(IList<CloudQueue> cloudQueues)
        {
            this.cloudQueues = cloudQueues ?? throw new ArgumentNullException(nameof(cloudQueues));
            this.roundRobbinNumberResolver = new RoundRobbinNumberResolver(cloudQueues.Count);
        }

        public Task SendAsync(StorageQueueMessage message, CancellationToken cancelationToken = default(CancellationToken))
        {
            var nextQueue = roundRobbinNumberResolver.GetNextNumber();

            return cloudQueues[nextQueue].AddMessageAsync(message.Data);
        }
    }
}