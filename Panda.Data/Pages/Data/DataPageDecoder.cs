namespace Panda.Data.Pages.Data;

public class DataPageDecoder : IPageDecoder<DataPage>
{
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