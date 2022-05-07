using System.Collections;

namespace UltimateBattle;

public class TaskManager : ITask
{
    public Process Process { get; set; }

    public IEnumerator Run()
    {
        while (true)
        {
            if (Process.Disposed) yield break;
            Console.Write("[TaskManager] Available commands: list, resume, pause, kill, exit: ");
            var command = Console.ReadLine();
            switch (command)
            {
                case "list":
                    foreach (var (_, process) in Process.Processes)
                    {
                        Console.WriteLine(process);
                    }

                    break;
                case "resume":
                {
                    Console.Write("Input process id to resume: ");
                    int id = int.Parse(Console.ReadLine()!);
                    Process.Status = ProcessStatus.Ready;
                    Process.Processes[id].Resume();
                    break;
                }
                case "pause":
                    yield return null;
                    break;
                case "kill":
                {
                    Console.Write("Input process id to kill: ");
                    int id = int.Parse(Console.ReadLine()!);
                    Process.Processes[id].Dispose();
                    break;
                }
                case "exit":
                    yield break;
            }
        }
    }
}