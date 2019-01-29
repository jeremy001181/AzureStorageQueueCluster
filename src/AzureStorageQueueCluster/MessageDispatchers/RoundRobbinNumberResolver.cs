using System.Threading;

namespace AzureStorageQueueCluster.MessageDispatchers
{
    internal class RoundRobbinNumberResolver
    {
        private int maxNumbers;
        private int lastNumber;

        public RoundRobbinNumberResolver(int maxNumbers)
        {
            this.maxNumbers = maxNumbers;
        }

        public int GetNextNumber()
        {
            uint nextNumber = unchecked((uint)Interlocked.Increment(ref lastNumber));
            int result = (int)(nextNumber % maxNumbers);
            return result;
        }
    }
}