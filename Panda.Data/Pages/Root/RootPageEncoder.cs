namespace Panda.Data.Pages.Root;

public class RootPageEncoder : IPageEncoder<RootPage>
{
    const int rootPageDataOffset = 273;
    public void EncodePage(RootPage page, Memory<byte> buffer)
    {
        if (buffer.Length < page.PageSize) throw new ArgumentException("PageSize must exactly match the page size of the root page.", nameof(buffer));
        if (page.PageData == null) page = page with { PageData = Array.Empty<byte>() };
        if (page.PageData.Length > (buffer.Length - rootPageDataOffset))
            throw new InvalidOperationException("This pages data exceeds the page size and cannot be written.");
        var span = buffer.Span;

        span[0] = (byte)page.PageType;
        span[1] = page.FileVersion;
        span[2] = page.PageSizePower;
        WriteToSpan(BitConverter.GetBytes(page.Checksum), span[3..]);
        WriteToSpan(BitConverter.GetBytes(page.PageData.Length), span[7..]);
        WriteToSpan(BitConverter.GetBytes(page.FreePageIndexPageNumber), span[257..]);
        WriteToSpan(BitConverter.GetBytes(page.RootContinuationPageNumber), span[265..]);
        WriteToSpan(page.PageData, span[rootPageDataOffset..]);
    }

    public void WriteToSpan(byte[] data, Span<byte> span)
    {
        for (var x = 0; x < data.Length; x++) span[x] = data[x];
    }
}