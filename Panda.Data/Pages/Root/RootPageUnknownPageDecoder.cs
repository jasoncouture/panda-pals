using Panda.Data.Pages.Root;

namespace Panda.Data.Pages;

public class RootPageUnknownPageDecoder : UnknownPageDecoderBase<RootPage>
{
    public RootPageUnknownPageDecoder(IPageDecoder<RootPage> pageDecoder) : base(pageDecoder)
    {
    }
}