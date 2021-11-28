namespace Panda.Data.Pages.Root;

public record struct BranchPagePointer(Guid Start, Guid End, ulong PagePointer)
{
    // 32 is guid + guid (16 + 16)
    private const int GuidSize = 16;
    public const int Size = (GuidSize * 2) + sizeof(ulong);
    public static bool TryParseFromSpan(ReadOnlySpan<byte> span, out BranchPagePointer branchPagePointer)
    {
        branchPagePointer = default;
        if (span.Length < Size) return false;
        var start = new Guid(span[..16]);
        var end = new Guid(span[16..][..16]);
        var pagePointer = BitConverter.ToUInt64(span[32..][..8]);
        branchPagePointer = new BranchPagePointer(start, end, pagePointer);
        return true;
    } 
    public bool IsValid => PagePointer != 0 && Start != End;

    public void WriteTo(Span<byte> span)
    {
        CopyBytes(Start.ToByteArray(), span);
        CopyBytes(End.ToByteArray(), span[GuidSize..]);
        CopyBytes(BitConverter.GetBytes(PagePointer), span[(GuidSize*2)..]);
    }

    private static void CopyBytes(IReadOnlyList<byte> toByteArray, Span<byte> span)
    {
        for (var x = 0; x < toByteArray.Count; x++)
        {
            span[x] = toByteArray[x];
        }
    }
}