namespace Panda.Data.Pages.Free;

public class FreePageEncoder : IPageEncoder<FreePage>
{
    public void EncodePage(FreePage page, Memory<byte> buffer)
    {
        var span = buffer.Span;
        span[0] = (byte) page.PageType;
        if (!page.Zero) return; // Not zeroing a page will be faster
        span[1..].Clear(); // But some pages we may want zeroed.
    }
}