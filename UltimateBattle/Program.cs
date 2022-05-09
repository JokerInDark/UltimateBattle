using System.Diagnostics;
using UltimateBattle;
using Process = UltimateBattle.Process;

internal class Program
{
    private static MemoryManager _memoryManager;

    private static void Main(string[] args)
    {
        Console.Write("Input memory size: ");
        var memorySize = int.Parse(Console.ReadLine()!);
        PhysicalMemory physicalMemory = new(memorySize);
        Console.Write("Input page size: ");
        var pageSize = int.Parse(Console.ReadLine()!);
        Console.Write("Input swap size: ");
        var swapSize = int.Parse(Console.ReadLine()!);
        SwapSpace swapSpace = new(swapSize);
        _memoryManager = new MemoryManager(physicalMemory, pageSize,swapSpace);
        while (true)
        {
            Console.Write("[Main] Available commands: start, test, task, break, exit: ");
            var command = Console.ReadLine();
            switch (command)
            {
                case "start":
                    Console.Write("Input file name: ");
                    Process.Start(Console.ReadLine()!, _memoryManager);
                    break;
                case "test":
                    Process.Start(new TestTool(), _memoryManager);
                    break;
                case "task":
                    Process.Start(new TaskManager(),_memoryManager);
                    break;
                case "break":
                    Debugger.Break();
                    break;
                case "exit":
                    return;
            }
        }
    }
}