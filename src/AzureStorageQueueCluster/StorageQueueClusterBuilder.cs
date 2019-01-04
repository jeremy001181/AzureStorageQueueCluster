using System.Linq;
using System.Net;
using Microsoft.WindowsAzure.Storage;
using AzureStorageQueueCluster.Config;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AzureStorageQueueCluster
{
    class StorageQueueClusterBuilder : IStorageQueueClusterBuilder
    {
        private IConfigStore configStore;

        public StorageQueueClusterBuilder(IConfigStore configStore)
        {
            this.configStore = configStore;
        }

        public IStorageQueueCluster Build()
        {
            var configJson = configStore.GetConfig();

            var config = JsonConvert.DeserializeObject<List<StorageAccountConfig>>(configJson);

            var allCloudQueues = config.SelectMany(storageAccount =>
            {
                var account = CloudStorageAccount.Parse(storageAccount.ConnectionString);

                ServicePointManager.FindServicePoint(account.QueueEndpoint).UseNagleAlgorithm = false;

                var queueClient = account.CreateCloudQueueClient();

                var cloudQueues = storageAccount.Queues
                                        .Select(queue => queueClient.GetQueueReference(queue.Name))
                                        .ToList();
                return cloudQueues;
            }).ToList();

            return new StorageQueueCluster(allCloudQueues);
        }
    }
}
