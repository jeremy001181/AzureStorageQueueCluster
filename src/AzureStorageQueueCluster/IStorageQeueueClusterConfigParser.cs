using AzureStorageQueueCluster.Config;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Collections.Generic;

namespace AzureStorageQueueCluster
{
    internal interface IStorageQeueueClusterConfigParser
    {
        IReadOnlyList<CloudQueue> Parse(StorageQueueClusterConfig config);
    }
}