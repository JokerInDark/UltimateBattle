namespace UltimateBattle;

public class MemoryManager
{
    private readonly int _blocks;
    private readonly bool[] _freeBlockTable;
    private readonly int _pageSize;
    private readonly PhysicalMemory _physicalMemory;
    private readonly Queue<int> _swapQueue;
    private readonly SwapSpace _swapSpace;
    private readonly TranslationInfo[] _translationTable;

    public MemoryManager(PhysicalMemory physicalMemory, int pageSize, SwapSpace swapSpace)
    {
        _pageSize = pageSize;
        _swapSpace = swapSpace;
        _physicalMemory = physicalMemory;
        var physicalSize = physicalMemory.Size;
        var physicalBlocks = physicalMemory.Size / pageSize;
        var swapSize = swapSpace.Size;
        var swapBlocks = swapSpace.Size / pageSize;
        _blocks = physicalBlocks + swapBlocks;
        _freeBlockTable = new bool[_blocks];
        _translationTable = new TranslationInfo[_blocks];
        Array.Fill(_freeBlockTable, true);
        for (int i = 0; i < physicalBlocks; i++)
        {
            _translationTable[i] = new TranslationInfo
            {
                Position = i,
                MemoryType = MemoryType.Physical
            };
        }

        for (int i = 0; i < swapBlocks; i++)
        {
            _translationTable[i + physicalBlocks] = new TranslationInfo
            {
                Position = i,
                MemoryType = MemoryType.Swap
            };
        }

        var queueCapacity = Math.Min(physicalBlocks, swapBlocks);
        _swapQueue = new Queue<int>(queueCapacity);
        for (int i = physicalBlocks - queueCapacity; i < physicalBlocks; i++)
        {
            _swapQueue.Enqueue(i);
        }
    }

    public int GetPhysicalAddress(int logicalAddress, List<int> pageTable)
    {
        var page = logicalAddress / _pageSize;
        var offset = logicalAddress % _pageSize;
        var virtualBlock = pageTable[page];
        var translationInfo = _translationTable[virtualBlock];
        var physicalBlock = translationInfo.MemoryType switch
        {
            MemoryType.Physical => translationInfo.Position,
            MemoryType.Swap => SwapBlock(virtualBlock),
            _ => throw new ArgumentOutOfRangeException()
        };
        return physicalBlock * _pageSize + offset;
    }

    public int SwapBlock(int blockToSwapIn)
    {
        var blockToSwapOut = _swapQueue.Dequeue();
        ref var blockToSwapOutInfo = ref _translationTable[blockToSwapOut];
        ref var blockToSwapInInfo = ref _translationTable[blockToSwapIn];
        byte[] temp = new byte[_pageSize];
        var spanToSwapOut = _physicalMemory.GetSpan(blockToSwapOutInfo.Position * _pageSize, _pageSize);
        var spanToSwapIn = _swapSpace.GetSpan(blockToSwapInInfo.Position * _pageSize, _pageSize);
        spanToSwapOut.CopyTo(temp);
        spanToSwapIn.CopyTo(spanToSwapOut);
        temp.CopyTo(spanToSwapIn);
        _swapQueue.Enqueue(blockToSwapIn);
        (blockToSwapOutInfo, blockToSwapInInfo) = (blockToSwapInInfo, blockToSwapOutInfo);
        return blockToSwapInInfo.Position;
    }

    public T Get<T>(int logicalAddress, List<int> pageTable) where T : struct
    {
        var physicalAddress = GetPhysicalAddress(logicalAddress, pageTable);
        return _physicalMemory.GetReference<T>(physicalAddress);
    }

    public void Set<T>(int logicalAddress, List<int> pageTable, T value) where T : struct
    {
        var physicalAddress = GetPhysicalAddress(logicalAddress, pageTable);
        _physicalMemory.GetReference<T>(physicalAddress) = value;
    }

    public int Allocate(int length, List<int> pageTable)
    {
        var pages = (int) Math.Ceiling((double) length / _pageSize);
        var start = pageTable.Count;
        for (int i = 0, j = 0; i < pages; i++)
            while (true)
            {
                if (j == _blocks) throw new Exception("Out of Memory");

                if (_freeBlockTable[j])
                {
                    _freeBlockTable[j] = false;
                    pageTable.Add(j);
                    j++;
                    break;
                }

                j++;
            }

        return start * _pageSize;
    }

    public void Free(int address, List<int> pageTable)
    {
        var pages = pageTable.Count - address / _pageSize;
        for (var i = 0; i < pages; i++)
        {
            var block = pageTable.Last();
            _freeBlockTable[block] = true;
            pageTable.RemoveAt(pageTable.Count - 1);
        }
    }
}