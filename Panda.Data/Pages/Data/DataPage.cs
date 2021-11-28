namespace Panda.Data.Pages.Data;


[PageType(PageType.Data)]
public record DataPage(ulong NextDataPage, uint Checksum, byte[] Data) : IPage<DataPage>
{
    public PageType PageType => PageType.Data;
}