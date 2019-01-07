using System.Collections.Generic;

namespace AzureStorageQueueCluster.Config
{
    public class StorageAccountConfig
    {
        public string ConnectionString { get; set; }
        public IList<QueueConfig> Queues { get; set; }
    }
}