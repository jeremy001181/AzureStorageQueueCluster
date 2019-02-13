using System.Collections.Generic;

namespace AzureStorageQueueCluster.Config
{
    public class StorageQueueClusterConfig {
        public IList<StorageAccountConfig> StorageAccounts { get; set; }
        public DispatchMode DispatchMode { get; set; } = DispatchMode.ActivePassive;
    }
}