using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.FileAccessModule.StatisticsInfo
{
    internal class MemoryCacheInfo
    {
        private int memoryEntryQuantity;
        private long memoryTotalSize;
        private int memoryEntryHistoryQuantity;
        private long memoryHistoryTotalSize;

        public int MemoryEntryQuantity { get => memoryEntryQuantity; }
        public long MemoryTotalSize { get => memoryTotalSize; }
        public int MemoryEntryHistoryQuantity { get => memoryEntryHistoryQuantity; }
        public long MemoryHistoryTotalSize { get => memoryHistoryTotalSize; }

        public void AddMemoryEnTry(long memoryEntrySize)
        {
            Interlocked.Increment(ref memoryEntryHistoryQuantity);
            Interlocked.Increment(ref memoryEntryQuantity);
            Interlocked.Add(ref memoryTotalSize, memoryEntrySize);
            Interlocked.Add(ref memoryHistoryTotalSize, memoryEntrySize);
        }

        public void RemoveMemoryEntry(long memoryEntrySize)
        {
            Interlocked.Decrement(ref memoryEntryQuantity);
            Interlocked.Add(ref memoryTotalSize, (0-memoryEntrySize));
        }
    }
}
