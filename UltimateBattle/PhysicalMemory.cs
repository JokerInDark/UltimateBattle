using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltimateBattle;

public class PhysicalMemory
{
    public byte[] Memory;

    public PhysicalMemory(int bytes)
    {
        Memory = new byte[bytes];
    }

    public int Size => Memory.Length;

    public ref T GetReference<T>(int address) where T : struct
    {
        var span = new Span<byte>(Memory, address, Unsafe.SizeOf<T>());
        var cast = MemoryMarshal.Cast<byte, T>(span);
        return ref cast.GetPinnableReference();
    }
}