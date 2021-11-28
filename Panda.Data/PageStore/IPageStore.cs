namespace Panda.Data.PageStore;

public interface IPageStore
{
    IReadOnlyPageBlock OpenReadOnlyPage(ulong pageNumber);
    IPageBlock OpenPage(ulong pageNumber);
    void WritePage(byte[] buffer, ulong pageNumber);
}