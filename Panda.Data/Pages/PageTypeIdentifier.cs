namespace Panda.Data.Pages;

public class PageTypeIdentifier : IPageTypeIdentifier
{
    private readonly HashSet<PageType> _knownPageTypes;

    public PageTypeIdentifier()
    {
        _knownPageTypes = Enum.GetValues<PageType>().ToHashSet();
    }
    public PageType DeterminePageType(ReadOnlyMemory<byte> memory)
    {
        var pageType = (PageType) memory.Span[0];
        return !_knownPageTypes.Contains(pageType) ? PageType.Unknown : pageType;
    }
}