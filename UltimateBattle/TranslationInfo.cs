namespace UltimateBattle;

public struct TranslationInfo
{
    public int Position;
    public MemoryType MemoryType;
}

public enum MemoryType
{
    Physical,
    Swap
}