using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using AzureStorageQueueCluster.Config;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Moq;
using Newtonsoft.Json;

namespace AzureStorageQueueCluster.Tests.IntegrationTests
{
    public abstract class BaseTest
    {
        protected readonly Mock<CloudQueueClient> queueClient;
        protected readonly Mock<CloudStorageAccount> cloudStorageAccout;
        private readonly Mock<ICloudStorageAccountParser> cloudStorageAccountParser;

        protected readonly StorageQueueClusterBuilder builder;
        protected StorageQueueClusterConfig config = new StorageQueueClusterConfig();

        protected BaseTest()
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

            //builder = new StorageQueueClusterBuilder(config, cloudStorageAccountParser.Object, new ConfigValidator());
        }
    }
}