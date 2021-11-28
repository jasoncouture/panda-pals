using System;
using System.Linq;
using Panda.Data.Pages.Free;
using Xunit;

namespace Panda.Data.UnitTests;

public class FreePageTests
{
    [Fact]
    public void FreePageOnlyWritesASingleByteWhenNotZeroingPage()
    {
        var freePage = new FreePage();
        var freePageEncoder = new FreePageEncoder();
        const byte expectedLeadByte = 0x00;
        const byte expectedUntouchedByte = 0xff;
        const byte expectedZeroedByte = 0x00;

        var page = new Memory<byte>(Enumerable.Range(0, 512).Select(i => (byte) 0xFF).ToArray());

        freePageEncoder.EncodePage(freePage, page);
        Assert.Equal(page.Span[0], expectedLeadByte);
        for (var x = 1; x < page.Span.Length; x++)
        {
            Assert.Equal(page.Span[x], expectedUntouchedByte);
        }

        freePage = freePage with {Zero = true};
        freePageEncoder.EncodePage(freePage, page);
        Assert.Equal(page.Span[0], expectedLeadByte);
        for (var x = 1; x < page.Span.Length; x++)
        {
            Assert.Equal(page.Span[x], expectedZeroedByte);
        }
    }

    [Fact]
    public void FreePageDecoderDoesntCareAboutPageSize()
    {
        var page = new Memory<byte>(Array.Empty<byte>());
        var freePageDecoder = new FreePageDecoder();

        var first = freePageDecoder.DecodePage(page);
        var second = freePageDecoder.DecodePage(page);

        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.Same(first, second);
    }
}