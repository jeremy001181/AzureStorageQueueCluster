using Microsoft.WindowsAzure.Storage;
using System;
using System.Threading.Tasks;

namespace AzureStorageQueueCluster
{
    internal static class ExceptionExtensions
    {
        internal static bool IsTransilient(this Exception exception) {
            if (exception is TaskCanceledException) {
                return true;
            }
            else {
                var storageException = exception as StorageException;

                if (storageException != null) {
                    return storageException.RequestInformation != null 
                        && storageException.RequestInformation.HttpStatusCode >= 500;
                }

                return exception.InnerException != null 
                    && exception.InnerException.GetType() == typeof(TaskCanceledException);
            }
        }
    }
}
