using System.Collections;

namespace UltimateBattle;

public interface ITask : IDisposable
{
    Process Process { get; set; }

    IEnumerator Run();

    void IDisposable.Dispose()
    {
    }
}