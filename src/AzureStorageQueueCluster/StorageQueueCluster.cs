using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureStorageQueueCluster
{
    internal class StorageQueueCluster : IStorageQueueCluster
    {
        private readonly IList<CloudQueue> cloudQueues;

        internal StorageQueueCluster(IList<CloudQueue> cloudQueues)
        {
            this.cloudQueues = cloudQueues;
        }
        
        public async Task AddMessageAsync(StorageQueueMessage message, CancellationToken cancelationToken = default(CancellationToken)) {
            await AddMessageRecusivelyAsync(0, message, cancelationToken);
        }

        private async Task AddMessageRecusivelyAsync(int index, StorageQueueMessage message, CancellationToken cancelationToken)
        {
            if (index >= cloudQueues.Count)
            {
                return;
            }
            var next = index + 1;
            var cloudQueue = cloudQueues[index];
            try
            {
                await cloudQueue.AddMessageAsync(message.Data, message.TimeToLive, message.InitialVisibilityDelay, message.Options, message.OperationContext,  cancelationToken);
            }
            catch (TaskCanceledException)
            {
                // Operation returned an invalid status code(anything other than 200 OK), 
                // republish on secondary topic
                //todo logging
                await AddMessageRecusivelyAsync(next, message, cancelationToken);
            }
            catch (Exception ex) when (ex.InnerException != null && ex.InnerException.GetType() == typeof(TaskCanceledException))
            {
                // Operation timeout
                //todo logging
                await AddMessageRecusivelyAsync(next, message, cancelationToken);
            }            
            catch (StorageException ex) 
                when (ex.RequestInformation != null && ex.RequestInformation.HttpStatusCode >= 500)
            {
                await AddMessageRecusivelyAsync(next, message, cancelationToken);
            }
        }
    }
}
