using System.Linq;
using System.Net;
using AzureStorageQueueCluster.Config;
using System;
using AzureStorageQueueCluster.MessageDispatchers;

namespace AzureStorageQueueCluster
{
    public class StorageQueueClusterBuilder : IStorageQueueClusterBuilder
    {
        private StorageQueueClusterConfig config;
        private readonly IStorageQeueueClusterConfigParser cloudQueueParser;

        public StorageQueueClusterBuilder(StorageQueueClusterConfig config) 
            : this (config, new StorageQeueueClusterConfigParser(new CloudStorageAccountParser()))
        {
        }

        internal StorageQueueClusterBuilder(StorageQueueClusterConfig config, IStorageQeueueClusterConfigParser cloudQueueParser)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.cloudQueueParser = cloudQueueParser ?? throw new ArgumentNullException(nameof(cloudQueueParser));
        }

        public IStorageQueueCluster Build()
        {
            var allCloudQueues = cloudQueueParser.Parse(config);

            if (allCloudQueues.Count == 0)
            {
                throw new ArgumentException("Cannot initialize cluster with no queue", nameof(allCloudQueues));
            }

            var messageDispatcher = config.DispatchMode == DispatchMode.ActivePassive
                ? new ActivePassiveMessageDispatcher(allCloudQueues) as IMessageDispatcher
                : new RoundRobinMessageDispatcher(allCloudQueues);

            return new StorageQueueCluster(messageDispatcher);
        }
    }
}
