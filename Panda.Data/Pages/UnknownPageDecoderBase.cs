using System.Reflection;

namespace Panda.Data.Pages;

public abstract class UnknownPageDecoderBase<TPage> : IUnknownPageDecoder where TPage : IPage<TPage>
{
    private readonly IPageDecoder<TPage> _pageDecoder;
    private HashSet<PageType>? _supportedPageTypes;

    protected virtual HashSet<PageType> SupportedPageTypes =>
        _supportedPageTypes ??= GenerateSupportedPageTypes();

    private static HashSet<PageType> GenerateSupportedPageTypes()
    {
        return typeof(TPage)
            .GetCustomAttributes<PageTypeAttribute>()
            .Select(i => i.PageType)
            .ToHashSet();
    }

    protected UnknownPageDecoderBase(IPageDecoder<TPage> pageDecoder)
    {
        _pageDecoder = pageDecoder;
    }
    public virtual bool CanDecode(PageType type)
    {
        return SupportedPageTypes.Contains(type);
    }

    public virtual IUnknownPage Decode(ReadOnlyMemory<byte> page)
    {
        return _pageDecoder.DecodePage(page);
    }
}