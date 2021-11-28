namespace Panda.Data.Pages;

public interface IPageDecoder<out T> where T : IPage<T>
{
    T DecodePage(ReadOnlyMemory<byte> page);
}