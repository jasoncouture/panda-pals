namespace Panda.Data.Pages.Data;

public class DataPageEncoder : IPageEncoder<DataPage>
{
    public void EncodePage(DataPage page, Memory<byte> buffer)
    {
        var span = buffer.Span;
        span[0] = (byte) page.PageType;
        span = span[1..];
        WriteBytes(BitConverter.GetBytes(page.NextDataPage), span);
        WriteBytes(BitConverter.GetBytes(page.Checksum), span[8..]);
        WriteBytes(BitConverter.GetBytes(page.Data.Length), span[12..]);
        WriteBytes(page.Data, span[16..]);
    }

    private void WriteBytes(byte[] data, Span<byte> target)
    {
        if (data.Length == 0) return;
        data.CopyTo(target);
    }
}