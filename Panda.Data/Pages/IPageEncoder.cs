namespace Panda.Data.Pages;

public interface IPageEncoder<in T> where T : IPage<T>
{
    void EncodePage(T page, Memory<byte> buffer);
}