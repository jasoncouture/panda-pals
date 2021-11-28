namespace Panda.Data.Pages.Data;

public class DataPageEncoder : IPageEncoder<DataPage>, IPageDecoder<DataPage>
{
    public void EncodePage(DataPage page, Memory<byte> buffer)
    {
        var span = buffer.Span[1..];
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

    public DataPage DecodePage(ReadOnlyMemory<byte> page)
    {
        var span = page.Span[1..];
        var nextDataPage = BitConverter.ToUInt64(span);
        var checksum = BitConverter.ToUInt32(span[8..]);
        var dataLength = BitConverter.ToInt32(span[12..]);
        var buffer = new byte[dataLength];

        span[16..][..buffer.Length].CopyTo(buffer);

        return new DataPage(nextDataPage, checksum, buffer);
    }
}