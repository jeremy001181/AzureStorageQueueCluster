using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureStorageQueueCluster
{
    internal class ClusterQueue
    {
        private readonly CloudQueue cloudQueue;
        private readonly int backOffInSeconds;

        public ClusterQueue(CloudQueue cloudQueue, int backOffInSeconds = 2)
        {
            this.cloudQueue = cloudQueue;
            this.backOffInSeconds = backOffInSeconds;
        }

        public bool IsBusy { get; private set; }

        internal async Task AddMessageAsync(StorageQueueMessage message, CancellationToken cancelationToken)
        {
            try
            {
                await cloudQueue.AddMessageAsync(message.Data, message.TimeToLive, message.InitialVisibilityDelay, message.Options, message.OperationContext, cancelationToken);
            }
            catch (Exception ex) when (ex.IsTransilient())
            {
                IsBusy = true;
            }
        }
    }
}