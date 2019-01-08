using System.Linq;
using System.Net;
using AzureStorageQueueCluster.Config;
using System;

namespace AzureStorageQueueCluster
{
    public class StorageQueueClusterBuilder : IStorageQueueClusterBuilder
    {
        private StorageQueueClusterConfig config;
        private readonly ICloudStorageAccountParser cloudStorageAccountParser;
        private readonly IConfigValidator configValidator;

        public StorageQueueClusterBuilder(StorageQueueClusterConfig config) : this(config, new CloudStorageAccountParser(), new ConfigValidator())
        {
        }

        internal StorageQueueClusterBuilder(StorageQueueClusterConfig config
            , ICloudStorageAccountParser cloudStorageAccountParser
            , IConfigValidator configValidator)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            this.cloudStorageAccountParser = cloudStorageAccountParser;
            this.configValidator = configValidator;
        }

        public IStorageQueueCluster Build()
        {
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
