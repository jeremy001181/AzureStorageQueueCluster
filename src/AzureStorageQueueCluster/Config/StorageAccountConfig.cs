using System.Collections.Generic;

namespace AzureStorageQueueCluster.Config
{
    internal class StorageAccountConfig
    {
        internal string ConnectionString { get; set; }
        internal IEnumerable<QueueConfig> Queues { get; set; }
    }
}