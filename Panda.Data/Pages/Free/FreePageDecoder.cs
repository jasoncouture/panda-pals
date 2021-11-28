namespace Panda.Data.Pages.Free;

public class FreePageDecoder : IPageDecoder<FreePage>
{
    private static readonly FreePage DefaultFreePage = new ();
    public FreePage DecodePage(ReadOnlyMemory<byte> page)
    {
        // We cheat, because the caller should have already ensured this is a PageType.Free
        // So we actually don't do anything, because we don't care about the contents of free pages.
        // And to save memory, we reuse the same instance as well, since we always return "Clear = false"
        return DefaultFreePage;
    }
}