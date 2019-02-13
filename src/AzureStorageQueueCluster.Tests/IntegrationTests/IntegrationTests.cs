using AzureStorageQueueCluster.Config;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AzureStorageQueueCluster.Tests.IntegrationTests
{
    public class IntegrationTests : IDisposable
    {
        private readonly IStorageQueueCluster cluster;
        private readonly CloudQueue primary;
        private readonly CloudQueue secondary;

        public const string PRIMARY_QUEUE = "primary";
        public const string SECONDARY_QUEUE = "secondary";

        public IntegrationTests()
        {
            var queueClient = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudQueueClient();

            primary = queueClient.GetQueueReference(PRIMARY_QUEUE);
            secondary = queueClient.GetQueueReference(SECONDARY_QUEUE);

            primary.CreateIfNotExistsAsync();
            secondary.CreateIfNotExistsAsync();

            var clusterConfig = new StorageQueueClusterConfig()
            {
                StorageAccounts = new List<StorageAccountConfig>()
                {
                    new StorageAccountConfig()
                    {
                        ConnectionString = "UseDevelopmentStorage=true",
                        Queues = new List<QueueConfig>(){
                            new QueueConfig {
                                Name = PRIMARY_QUEUE
                            },
                            new QueueConfig {
                                Name = SECONDARY_QUEUE
                            }
                        }
                    }
                }
            };

            cluster = new StorageQueueClusterBuilder(clusterConfig)
                .Build();
        }

        [Fact]
        public async Task Should_send_message_to_secondary_if_primary_queue_not_available() {
            var message = new StorageQueueMessage(new CloudQueueMessage("{}"));
            await cluster.AddMessageAsync(message);
                        
            var messages = await secondary.GetMessagesAsync(32);

            Assert.Single(messages);
        }

        public void Dispose()
        {
            primary?.DeleteIfExistsAsync();
            secondary?.DeleteIfExistsAsync();
        }
    }
}
