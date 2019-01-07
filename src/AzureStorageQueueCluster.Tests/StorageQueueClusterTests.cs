using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Moq;
using Xunit;

namespace AzureStorageQueueCluster.Tests
{
    public class StorageQueueClusterTests
    {
        private readonly StorageQueueMessage message = new StorageQueueMessage(new CloudQueueMessage("test"));
        private readonly Mock<CloudQueue> queue1 = new Mock<CloudQueue>(new Uri("http://queue1"));
        private readonly Mock<CloudQueue> queue2 = new Mock<CloudQueue>(new Uri("http://queue2"));
        private readonly Mock<CloudQueue> queue3 = new Mock<CloudQueue>(new Uri("http://queue3"));
        private readonly IList<CloudQueue> cloudQueues;
        private readonly StorageQueueCluster cluster;

        public StorageQueueClusterTests()
        {
            cloudQueues = new List<CloudQueue>() { queue1.Object, queue2.Object, queue3.Object };        
            cluster = new StorageQueueCluster(cloudQueues);
        }
        [Fact]
        public void Should_throw_arugment_exception_when_initialize_empty_queue() {
            Assert.Throws<ArgumentException>(() => new StorageQueueCluster(null));
            Assert.Throws<ArgumentException>(() => new StorageQueueCluster(new List<CloudQueue>()));
        }

        [Fact]
        public async Task Should_send_message_via_the_first_queue_in_the_cluster()
        {
            await cluster.AddMessageAsync(message);

            queue1.Verify(AddMessageAsync(message), Times.Once);
            queue2.Verify(AddMessageAsync(message), Times.Never);
            queue3.Verify(AddMessageAsync(message), Times.Never);
        }

        [Fact]
        public async Task Should_send_message_via_the_second_queue_when_the_first_queue_throw_TaskCanceledException()
        {
            queue1.Setup(AddMessageAsync(message)).Throws<TaskCanceledException>();

            await cluster.AddMessageAsync(message);

            queue1.Verify(AddMessageAsync(message), Times.Once);
            queue2.Verify(AddMessageAsync(message), Times.Once);
            queue3.Verify(AddMessageAsync(message), Times.Never);
        }


        [Fact]
        public async Task Should_send_message_via_the_third_queue_when_both_the_first_and_second_queues_throw_TaskCanceledException()
        {
            queue1.Setup(AddMessageAsync(message)).Throws<TaskCanceledException>();
            queue2.Setup(AddMessageAsync(message)).Throws<TaskCanceledException>();

            await cluster.AddMessageAsync(message);

            queue1.Verify(AddMessageAsync(message), Times.Once);
            queue2.Verify(AddMessageAsync(message), Times.Once);
            queue3.Verify(AddMessageAsync(message), Times.Once);
        }


        [Fact]
        public async Task Should_throw_exception_when_failed_to_send_messages_via_entire_cluster()
        {
            queue1.Setup(AddMessageAsync(message)).Throws<TaskCanceledException>();
            queue2.Setup(AddMessageAsync(message)).Throws<TaskCanceledException>();
            queue3.Setup(AddMessageAsync(message)).Throws<TaskCanceledException>();

            await Assert.ThrowsAsync<TaskCanceledException>(async () => await cluster.AddMessageAsync(message));
        }

        private System.Linq.Expressions.Expression<Func<CloudQueue, Task>> AddMessageAsync(StorageQueueMessage message)
        {
            return self => self.AddMessageAsync(message.Data, message.TimeToLive, message.InitialVisibilityDelay, message.Options, message.OperationContext, It.IsAny<CancellationToken>());
        }
    }
}
