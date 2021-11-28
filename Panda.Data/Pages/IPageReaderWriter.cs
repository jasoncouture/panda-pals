using System.Collections.Concurrent;

namespace Panda.Data.Pages;

public interface IPageReader
{
    IUnknownPage? ReadPage(ReadOnlyMemory<byte> page);
}

public class PageReader : IPageReader
{
    private readonly IEnumerable<IUnknownPageDecoder> _unknownPageDecoders;
    private readonly IPageTypeIdentifier _pageTypeIdentifier;
    private readonly ConcurrentDictionary<PageType, IUnknownPageDecoder?> _pageDecoderCache = new();
    public PageReader(IEnumerable<IUnknownPageDecoder> unknownPageDecoders, IPageTypeIdentifier pageTypeIdentifier)
    {
        _unknownPageDecoders = unknownPageDecoders;
        _pageTypeIdentifier = pageTypeIdentifier;
    }

    private IUnknownPageDecoder? LocateFirstDecoder(PageType pageType)
    {
        if (_pageDecoderCache.TryGetValue(pageType, out var decoder)) return decoder;
        foreach (var pageDecoder in _unknownPageDecoders)
        {
            if (!pageDecoder.CanDecode(pageType)) continue;
            decoder = pageDecoder;
            break;
        }

        _pageDecoderCache.TryAdd(pageType, decoder);
        return decoder;
    }

    public IUnknownPage? ReadPage(ReadOnlyMemory<byte> page)
    {
        // First up, what kind of page is this??
        var pageType = _pageTypeIdentifier.DeterminePageType(page);
        // Try to grab the decoder for this type
        var decoder = LocateFirstDecoder(pageType);
        // And if we got a decoder, decode, otherwise return null.
        // TODO: Update this to decode a "default page" with an unknown page type, and filled with bytes)
        // Possibly by adding an UnknownPage (And decoders?)
        return decoder?.Decode(page);
    }
}