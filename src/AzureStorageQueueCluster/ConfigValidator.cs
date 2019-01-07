using AzureStorageQueueCluster.Config;
using System;

namespace AzureStorageQueueCluster
{
    internal class ConfigValidator : IConfigValidator {
        public void EnsureValidConfiguration(StorageQueueClusterConfig config) {
            if (config == null) {
                throw new ArgumentNullException("Storage queue cluster config cannot be null");
            }

            if (config.StorageAccounts == null || config.StorageAccounts.Count == 0)
            {
                throw new InvalidOperationException("No storage account specified in config");
            }
        }
    }

    internal interface IConfigValidator
    {
        void EnsureValidConfiguration(StorageQueueClusterConfig config);
    }
}