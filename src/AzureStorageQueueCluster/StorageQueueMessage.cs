using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureStorageQueueCluster
{
    public class StorageQueueMessage
    {
        public StorageQueueMessage(CloudQueueMessage message)
        {
            Data = message ?? throw new ArgumentNullException(nameof(message));
        }
        internal CloudQueueMessage Data { get; }
        public TimeSpan? TimeToLive { get; set; }
        public TimeSpan? InitialVisibilityDelay { get; set; }
        public QueueRequestOptions Options { get; set; }
        public OperationContext OperationContext { get; set; }
    }
}