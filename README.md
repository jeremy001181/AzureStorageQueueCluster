# AzureStorageQueueCluster
Supports sending messages to a cluster of queues via two different dispatch mode that is configurable.
1. Active and passive, all messages will be sent to the first queue in the list as primary, messages will be automatically retry on others queues (secondary) in the list if failed to be delivered to primary queue.
2. Round robin, all messages will sent to all configured queues using a round robin algorithm. 

# How to use

```C#
var cluster = new StorageQueueClusterBuilder(new StorageQueueClusterConfig(){	
	StorageAccounts = new List<StorageAccountConfig>() {
                new StorageAccountConfig(){
                    ConnectionString = "conn1",
                    Queues = new List<QueueConfig>(){
                        new QueueConfig{
                            Name = "queue1"
                        },
                        new QueueConfig{
                            Name = "queue2"
                        }
                    }
                },
				new StorageAccountConfig(){
                    ConnectionString = "conn2",
                    Queues = new List<QueueConfig>(){
                        new QueueConfig{
                            Name = "queue3"
                        },
                        new QueueConfig{
                            Name = "queue4"
                        }
                    }
                }
    }).Build();

await cluster.AddMessageAsync(new StorageQueueMessage(new CloudQueueMessage("msg")));
```
