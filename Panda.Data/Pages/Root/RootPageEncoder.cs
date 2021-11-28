namespace Panda.Data.Pages.Root;

public class RootPageDecoder : IPageDecoder<RootPage>
{
    const int rootPageDataOffset = 273;
    public RootPage DecodePage(ReadOnlyMemory<byte> page)
    {
        var span = page.Span;
        var pageFileVersion = span[1];
        var pageSizePower = span[2];
        var temporaryRootPage = new RootPage(pageFileVersion, pageSizePower, 0, 0, 0, Array.Empty<byte>());

        var pageSize = temporaryRootPage.PageSize;
        if (pageSize > page.Length)
        {
            return temporaryRootPage with { PageData = null, PageSizeChangeNeeded = true };
        }

        // Good to go!
        var pageChecksum = BitConverter.ToUInt32(span[3..]);
        var pageBodyBytes = BitConverter.ToInt32(span[7..]);
        var pageFreePageIndexPageNumber = BitConverter.ToUInt64(span[257..]);
        var pageRootContinuationPageNumber = BitConverter.ToUInt64(span[265..]);
        var pagePageBody = Array.Empty<byte>();
        // Does our length make sense?
        if (pageBodyBytes > 0 && (pageBodyBytes + rootPageDataOffset) <= span.Length)
        {
            pagePageBody = new byte[pageBodyBytes];
            Array.Copy(span[rootPageDataOffset..].ToArray(), pagePageBody, pagePageBody.Length);
        }
        else if (pageBodyBytes != 0)
        {
            pageChecksum = 0; // Either pageBodyBytes is negative, or larger than the bytes remaining in a page
            // Either way, we can not trust it, and must rebuild it. Setting this to zero will cause the checksum check to fail
            // and the root page should be rewritten by the page handling logic.
            // (Or we can try to recover it from page 1, if page 1 is valid)

            // But either way, our job is to load the page, not do things with it.
        }

        return temporaryRootPage with
        {
            Checksum = pageChecksum,
            FreePageIndexPageNumber = pageFreePageIndexPageNumber,
            RootContinuationPageNumber = pageRootContinuationPageNumber,
            PageData = pagePageBody
        };
    }
}

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