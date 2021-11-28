namespace Panda.Data.Pages.Root;

[PageType(PageType.Root)]
public record RootPage(byte FileVersion, byte PageSizePower, uint Checksum, ulong FreePageIndexPageNumber, ulong RootContinuationPageNumber, byte[]? PageData, bool PageSizeChangeNeeded = false) : IPage<RootPage>
{
    // PageType.Root (1) Must be at page 0, and mirrored at page 1
    // This page Describes metadata about the file
    // Bytes:
    // 1 - File version (1 byte) (0-255, current: 0)
    // 2 - Page size power (1 byte) (0-16, varies, page size is 512 * Math.Pow(2, PageSizePower)
    //     IE: 0 = 512, 1 = 1024, 2 = 2048, 3 = 4096, 4 = 8192, 5 = 16384, 6 = 32768, 7 = 65536, 8 = 131072, 9 = 262144 and so on. 5 is the default. Values past 10 are not recommended.
    // 3 - 6 - 32bit CRC 32 of the data of this page (CRC32 is computed starting at byte 257 for bodyBytes count of bytes.
    // 7 - 10 - number of bytes contained in body.
    // 11 - 256 (RESERVED)
    // 257-264 - pointer to free page root page - If this is zero, the engine will stop what it's doing, scan the file and mark all free page indexes as free, and begin rebuilding the free page index.
    //         - The data file cannot be written until the free page index is rebuilt.
    // 265-272 - pointer to root continuation page, 0 if no continuation page
    // 273+ repeats the following structure:
    //  16 byte start key, 16 byte end key, page pointer to branch start for range (Also 8 bytes)
    // If the start key == the end key, this record is not valid.
    // This page should not change often, or really ever. It's meant to be built on DB init, and generally left alone.
    public int PageSize => (int)(Math.Pow(2, PageSizePower) * 512);

    // This isn't cached, because the engine should only be using this on startup to track the root branches.
    // After startup, this field is write only. (In theory)
    public IEnumerable<BranchPagePointer> GetBranchPagePointers()
    {
        var memory = new ReadOnlyMemory<byte>(PageData ?? Array.Empty<byte>());
        List<BranchPagePointer> pagePointers = new();
        var span = memory.Span;
        while (true)
        {
            if (!BranchPagePointer.TryParseFromSpan(span, out var next))
            {
                break;
            }

            span = span[BranchPagePointer.Size..];
            if (next.IsValid) pagePointers.Add(next);
        }

        return pagePointers;
    }

    public RootPage WithPagePointers(IEnumerable<BranchPagePointer> pagePointers)
    {
        var pagePointersToWrite = pagePointers.Where(i => i.IsValid).ToArray();
        if (pagePointersToWrite.Length == 0) return this with {PageData = Array.Empty<byte>()};
        var memory = new Memory<byte>(new byte[pagePointersToWrite.Length * BranchPagePointer.Size]);
        var span = memory.Span;

        foreach (var pagePointer in pagePointersToWrite)
        {
            pagePointer.WriteTo(span);
            span = span[BranchPagePointer.Size..];
        }

        return this with {PageData = memory.ToArray()};
    }

    public PageType PageType => PageType.Root;
}