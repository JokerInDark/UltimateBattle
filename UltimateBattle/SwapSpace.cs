namespace UltimateBattle;

// The swap space is also physical memory, interesting...
public class SwapSpace : PhysicalMemory
{
    public SwapSpace(int bytes) : base(bytes)
    {
    }
}