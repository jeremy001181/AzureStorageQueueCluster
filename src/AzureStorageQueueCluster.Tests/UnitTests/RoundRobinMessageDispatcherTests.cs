using AzureStorageQueueCluster.MessageDispatchers;
using Microsoft.WindowsAzure.Storage.Queue;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AzureStorageQueueCluster.Tests.UnitTests
{
    public class RoundRobinMessageDispatcherTests
    {
        [Fact]
        public async Task Should_equally_distribute_messages_cross_all_queues() {
            var uri = new Uri("http://queue1");
            var queue1 = new Mock<CloudQueue>(uri);
            var queue2 = new Mock<CloudQueue>(uri);
            var queue3 = new Mock<CloudQueue>(uri);

            var roundRobinDispatcher = new RoundRobinMessageDispatcher(new List<CloudQueue>() {
                queue1.Object, queue2.Object, queue3.Object
            });

            var tasks = new int[30].Select((i) => Task.Run(async () => {
                await roundRobinDispatcher.SendAsync(new StorageQueueMessage(new CloudQueueMessage("test")));
            }));

            await Task.WhenAll(tasks);

            queue1.Verify(self => self.AddMessageAsync(It.IsAny<CloudQueueMessage>()), Times.Exactly(10));
            queue2.Verify(self => self.AddMessageAsync(It.IsAny<CloudQueueMessage>()), Times.Exactly(10));
            queue3.Verify(self => self.AddMessageAsync(It.IsAny<CloudQueueMessage>()), Times.Exactly(10));
        }
    }
}
