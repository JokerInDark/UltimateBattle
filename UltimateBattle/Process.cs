using System.Collections;
using System.Reflection;

namespace UltimateBattle;

public class Process : IDisposable
{
    public static int LastId = -1;
    public static Dictionary<Assembly, ConstructorInfo> Cache = new();
    public static Dictionary<int, Process> Processes = new();
    public IEnumerator Context;
    public bool Disposed;
    public int Id;
    public MemoryManager MemoryManager;
    public List<int> PageTable = new();
    public ProcessStatus Status = ProcessStatus.Ready;
    public ITask Task;

    public void Dispose()
    {
        if (Disposed) return;
        Task.Dispose();
        Free(0);
        Processes.Remove(Id);
        Disposed = true;
    }

    public void Resume()
    {
        Status = ProcessStatus.Running;
        var ended = !Context.MoveNext();
        Status = ProcessStatus.Ready;
        if (ended) Dispose();
    }

    public static void Start(string fileName, MemoryManager memoryManager)
    {
        var assembly = Assembly.LoadFrom(fileName);
        Cache.TryGetValue(assembly, out var constructor);
        if (constructor == null)
        {
            var type = assembly.GetTypes().First(type => type.IsAssignableTo(typeof(ITask)));
            constructor =
                type.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    CallingConventions.HasThis,
                    Array.Empty<Type>(),
                    null
                );
            Cache.Add(assembly, constructor!);
        }

        var task = (ITask) constructor!.Invoke(null);
        Start(task, memoryManager);
    }

    public static void Start(ITask task, MemoryManager memoryManager)
    {
        Process pcb = new()
        {
            MemoryManager = memoryManager,
            Task = task,
            Id = ++LastId,
            Context = task.Run()
        };
        task.Process = pcb;
        Processes.Add(pcb.Id, pcb);
        pcb.Resume();
    }

    public int Allocate(int length)
    {
        var address = MemoryManager.Allocate(length, PageTable);
        return address;
    }

    public void Free(int logicalAddress)
    {
        MemoryManager.Free(logicalAddress, PageTable);
    }

    public T Get<T>(int logicalAddress) where T : struct
    {
        return MemoryManager.Get<T>(logicalAddress, PageTable);
    }

    public void Set<T>(int logicalAddress, T value) where T : struct
    {
        MemoryManager.Set(logicalAddress, PageTable, value);
    }

    public override string ToString()
    {
        return
            $"{nameof(Task)}: {Task}, {nameof(Context)}: {Context}, {nameof(Id)}: {Id}, {nameof(MemoryManager)}: {MemoryManager}, {nameof(PageTable)}: {PageTable}, {nameof(Status)}: {Status}";
    }
}

public enum ProcessStatus
{
    Ready,
    Running
}