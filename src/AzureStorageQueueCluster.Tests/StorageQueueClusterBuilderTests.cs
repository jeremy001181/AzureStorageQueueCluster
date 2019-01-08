using AzureStorageQueueCluster.Config;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AzureStorageQueueCluster.Tests
{
    public partial class StorageQueueClusterBuilderTests
    {
        private readonly Mock<CloudQueueClient> queueClient;
        private readonly Mock<CloudStorageAccount> cloudStorageAccout;
        private readonly Mock<ICloudStorageAccountParser> cloudStorageAccountParser;
        private readonly StorageQueueClusterBuilder builder;
        private StorageQueueClusterConfig config = new StorageQueueClusterConfig();

        public StorageQueueClusterBuilderTests()
        {
            var storageUrl = "http://test.com";
            var endpoint = Convert.ToBase64String(Encoding.UTF8.GetBytes(storageUrl));
            var storageCredentials = new StorageCredentials(endpoint, endpoint);

            queueClient = new Mock<CloudQueueClient>(new Uri(storageUrl), storageCredentials)
            {
                CallBase = true
            };
            cloudStorageAccout = new Mock<CloudStorageAccount>(storageCredentials, false)
            {
                CallBase = true
            };
            cloudStorageAccout.Setup(self => self.CreateCloudQueueClient())
                              .Returns(queueClient.Object);

            cloudStorageAccountParser = new Mock<ICloudStorageAccountParser>();
            cloudStorageAccountParser.Setup(self => self.Parse(It.IsAny<string>()))
                                     .Returns(cloudStorageAccout.Object);
            
            builder = new StorageQueueClusterBuilder(config, cloudStorageAccountParser.Object, new ConfigValidator());
        }

        [Fact]
        public void Should_throw_InvalidOperationException_when_there_is_no_storage_account_in_config()
        {
            config = new StorageQueueClusterConfig();

            Assert.Throws<InvalidOperationException>(() => builder.Build());


            config = new StorageQueueClusterConfig() {
                StorageAccounts = new List<StorageAccountConfig>()
            };

            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public void Should_build_a_queue_cluster_correctly()
        {
            // arrange
            config.StorageAccounts = new List<StorageAccountConfig>() {
                               new StorageAccountConfig()
                               {
                                   ConnectionString = "conn",
                                   Queues = new List<QueueConfig>(){
                                       new QueueConfig{
                                           Name = "queue1"
                                       },
                                       new QueueConfig{
                                           Name = "queue2"
                                       }
                                   }
                               },
                               new StorageAccountConfig()
                               {
                                   ConnectionString = "conn2",
                                   Queues = new List<QueueConfig>(){
                                       new QueueConfig{
                                           Name = "queue3"
                                       }
                                   }
                               }
                           };

            // act
            var actual = builder.Build();

            // assert
            Assert.IsAssignableFrom<IStorageQueueCluster>(actual);
            cloudStorageAccout.Verify(self => self.CreateCloudQueueClient(), Times.Exactly(config.StorageAccounts.Count));

            foreach (var storageAccount in config.StorageAccounts)
            {
                cloudStorageAccountParser.Verify(self => self.Parse(storageAccount.ConnectionString), Times.Once);

                foreach (var queue in storageAccount.Queues)
                {
                    queueClient.Verify(self => self.GetQueueReference(queue.Name), Times.Once);
                }
            }
        }
    }
}
