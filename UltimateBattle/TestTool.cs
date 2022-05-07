using System.Collections;
using System.Diagnostics;

namespace UltimateBattle;

public class TestTool : ITask
{
    public Process Process { get; set; }

    public IEnumerator Run()
    {
        while (true)
        {
            if (Process.Disposed) yield break;
            Console.Write("[TestTool] Available commands: allocate, free, break, pause, exit: ");
            var command = Console.ReadLine();
            switch (command)
            {
                case "allocate":
                {
                    Console.Write("Input length to allocate: ");
                    int length = int.Parse(Console.ReadLine()!);
                    int address = Process.Allocate(length);
                    Console.WriteLine($"Allocated at {address}");
                    break;
                }
                case "free":
                {
                    Console.Write("Input address to free: ");
                    int address = int.Parse(Console.ReadLine()!);
                    Process.Free(address);
                    Console.WriteLine("Freed");
                    break;
                }
                case "break":
                    Debugger.Break();
                    break;
                case "pause":
                    yield return null;
                    break;
                case "exit":
                    yield break;
            }
        }
    }
}