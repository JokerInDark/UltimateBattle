using System.Collections;

namespace UltimateBattle;

public class MemoryManager
{
    private readonly int _blocks;
    private readonly List<bool> _freeBlockTable;
    private readonly PhysicalMemory _memory;
    private readonly int _pageSize;

    public MemoryManager(PhysicalMemory memory, int pageSize)
    {
        _pageSize = pageSize;
        _memory = memory;
        _blocks = memory.Size / pageSize;
        _freeBlockTable = new List<bool>(_blocks);
        for (int i = 0; i < _blocks; i++)
        {
            _freeBlockTable.Add(true);
        }
    }

    public int GetPhysicalAddress(int logicalAddress, List<int> pageTable)
    {
        var page = logicalAddress / _pageSize;
        var offset = logicalAddress % _pageSize;
        var physicalBlock = pageTable[page];
        return physicalBlock * _pageSize + offset;
    }

    public T Get<T>(int logicalAddress, List<int> pageTable) where T : struct
    {
        var physicalAddress = GetPhysicalAddress(logicalAddress, pageTable);
        return _memory.GetReference<T>(physicalAddress);
    }

    public void Set<T>(int logicalAddress, List<int> pageTable, T value) where T : struct
    {
        var physicalAddress = GetPhysicalAddress(logicalAddress, pageTable);
        _memory.GetReference<T>(physicalAddress) = value;
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