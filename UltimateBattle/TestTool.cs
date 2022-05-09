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
            Console.Write("[TestTool] Available commands: allocate, free, break, fill, pause, exit: ");
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
                case "fill":
                {
                    Console.Write("Input address to start filling: ");
                    int address = int.Parse(Console.ReadLine()!);
                    Console.Write("Input length to fill int: ");
                    int length = int.Parse(Console.ReadLine()!);
                    Console.Write("Input int data to fill: ");
                    int data = int.Parse(Console.ReadLine()!);
                    for (int i = 0; i < length; i++)
                    {
                        Process.Set(address + i * 4, data);
                    }

                    break;
                }
                case "pause":
                    yield return null;
                    break;
                case "exit":
                    yield break;
            }
        }
    }
}