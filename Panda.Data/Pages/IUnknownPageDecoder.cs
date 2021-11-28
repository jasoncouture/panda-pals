namespace Panda.Data.Pages;

public interface IUnknownPageDecoder
{
    bool CanDecode(PageType type);
    IUnknownPage Decode(ReadOnlyMemory<byte> page);
}