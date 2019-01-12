using AzureStorageQueueCluster.MessageDispatchers;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using Xunit;

namespace AzureStorageQueueCluster.Tests
{
    public class StorageQueueClusterTests
    {
        [Fact]
        public void Should_throw_arugment_exception_when_initialize_empty_queue()
        {
            Assert.Throws<ArgumentException>(() => new StorageQueueCluster(null, new ActivePassiveMessageDispatcher()));
            Assert.Throws<ArgumentException>(() => new StorageQueueCluster(new List<CloudQueue>(), new ActivePassiveMessageDispatcher()));
        }
    }
}
