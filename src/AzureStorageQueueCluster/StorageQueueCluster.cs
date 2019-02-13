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
        private readonly IMessageDispatcher sender;

        internal StorageQueueCluster(IMessageDispatcher sender)
        {
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }
        
        public async Task AddMessageAsync(StorageQueueMessage message, CancellationToken cancelationToken = default(CancellationToken)) {

            await sender.SendAsync(message, cancelationToken);

        }        
    }
}
