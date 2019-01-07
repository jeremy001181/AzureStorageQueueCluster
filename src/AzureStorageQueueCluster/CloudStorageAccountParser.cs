﻿using Microsoft.WindowsAzure.Storage;

namespace AzureStorageQueueCluster
{
    internal class CloudStorageAccountParser : ICloudStorageAccountParser
    {
        public CloudStorageAccount Parse(string connectionString)
        {
            return CloudStorageAccount.Parse(connectionString);
        }
    }

    internal interface ICloudStorageAccountParser
    {
        CloudStorageAccount Parse(string connectionString);
    }
}