using AzureStorageQueueCluster.Config;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace AzureStorageQueueCluster
{
    internal class StorageQeueueClusterConfigParser : IStorageQeueueClusterConfigParser
    {
        private readonly ICloudStorageAccountParser cloudStorageAccountParser;

        internal StorageQeueueClusterConfigParser(ICloudStorageAccountParser cloudStorageAccountParser)
        {
            this.cloudStorageAccountParser = cloudStorageAccountParser;
        }

        public IReadOnlyList<CloudQueue> Parse(StorageQueueClusterConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("Storage queue cluster config cannot be null");
            }

            if (config.StorageAccounts == null || config.StorageAccounts.Count == 0)
            {
                throw new InvalidOperationException("No storage account specified in config");
            }

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

            return allCloudQueues;
        }


        private static void OptimizeServicePoint(Uri queueEndpoint)
        {
            var servicePoint = ServicePointManager.FindServicePoint(queueEndpoint);

            servicePoint.UseNagleAlgorithm = false;
            if (servicePoint.ConnectionLimit < 200)
            {
                servicePoint.ConnectionLimit = 200;
            }
        }
    }
}