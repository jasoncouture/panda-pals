namespace Panda.Data.Pages;

public interface IPageReader
{
    IUnknownPage? ReadPage(ReadOnlyMemory<byte> page);
}