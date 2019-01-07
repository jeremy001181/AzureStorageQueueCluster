using System.Linq;
using System.Net;
using AzureStorageQueueCluster.Config;
using System;

namespace AzureStorageQueueCluster
{
    public class StorageQueueClusterBuilder : IStorageQueueClusterBuilder
    {
        private IConfigStore configStore;
        private readonly ICloudStorageAccountParser cloudStorageAccountParser;
        private readonly IConfigValidator configValidator;

        public StorageQueueClusterBuilder(IConfigStore configStore) : this(configStore, new CloudStorageAccountParser(), new ConfigValidator())
        {
        }

        internal StorageQueueClusterBuilder(IConfigStore configStore
            , ICloudStorageAccountParser cloudStorageAccountParser
            , IConfigValidator configValidator)
        {
            this.configStore = configStore ?? throw new ArgumentNullException(nameof(configStore));

            this.cloudStorageAccountParser = cloudStorageAccountParser;
            this.configValidator = configValidator;
        }

        public IStorageQueueCluster Build()
        {
            var config = configStore.GetConfig();

            configValidator.EnsureValidConfiguration(config);

            var allCloudQueues = config.StorageAccounts.SelectMany(storageAccount =>
            {
                var account = cloudStorageAccountParser.Parse(storageAccount.ConnectionString);
                OptimizeServicePoint(account.QueueEndpoint);

                var queueClient = account.CreateCloudQueueClient();

                var cloudQueues = storageAccount.Queues
                                        .Select(queue => queueClient.GetQueueReference(queue.Name))
                                        .ToList();
                return cloudQueues;
            }).ToList();

            return new StorageQueueCluster(allCloudQueues);
        }

        private static void OptimizeServicePoint(Uri queueEndpoint)
        {
            var servicePoint = ServicePointManager.FindServicePoint(queueEndpoint);

            servicePoint.UseNagleAlgorithm = false;
            if (servicePoint.ConnectionLimit < 200) {
                servicePoint.ConnectionLimit = 200;
            }
        }
    }
}
