namespace AzureStorageQueueCluster.Config
{
    public interface IConfigStore
    {
        StorageQueueClusterConfig GetConfig();
    }
}