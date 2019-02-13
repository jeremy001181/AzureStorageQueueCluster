using Microsoft.WindowsAzure.Storage;

namespace AzureStorageQueueCluster
{
    internal interface ICloudStorageAccountParser
    {
        CloudStorageAccount Parse(string connectionString);
    }
}