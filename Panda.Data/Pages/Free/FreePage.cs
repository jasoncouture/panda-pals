namespace Panda.Data.Pages.Free;

[PageType(PageType.Free)]
public record FreePage(bool Zero = false) : IPage<FreePage>
{
    public PageType PageType => PageType.Free;
}