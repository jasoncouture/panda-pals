using System;
using System.Linq;
using Panda.Data.Pages.Root;
using Xunit;
using Xunit.Abstractions;

namespace Panda.Data.UnitTests;

public class RootPageTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly BranchPagePointer[] _branches;

    public RootPageTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _branches = Enumerable.Range(0, 5)
            .Select(i => new BranchPagePointer(Guid.NewGuid(), Guid.NewGuid(), (ulong)(i + 1000)))
            .ToArray();
    }
    [Fact]
    public void RootPageEncodeDecodeTest()
    {
        var expectedRootPage = new RootPage(1, 0, 2, 3, 4, Array.Empty<byte>()).WithPagePointers(_branches);
        var rootPageEncoder = new RootPageEncoder();
        var rootPageDecoder = new RootPageDecoder();
        var page = new Memory<byte>(new byte[512]);

        rootPageEncoder.EncodePage(expectedRootPage, page);

        var actualRootPage = rootPageDecoder.DecodePage(page);
        Assert.NotNull(actualRootPage.PageData);
        Assert.NotEmpty(actualRootPage.PageData!);
        Assert.Equal(expectedRootPage.PageData, actualRootPage.PageData);
        Assert.Equal(expectedRootPage.FileVersion, actualRootPage.FileVersion);
        Assert.Equal(expectedRootPage.PageSizePower, actualRootPage.PageSizePower);
        Assert.Equal(expectedRootPage.Checksum, actualRootPage.Checksum);
        Assert.Equal(expectedRootPage.RootContinuationPageNumber, actualRootPage.RootContinuationPageNumber);
        Assert.Equal(expectedRootPage.FreePageIndexPageNumber, actualRootPage.FreePageIndexPageNumber);

        _testOutputHelper.WriteLine("Raw page data: {0}", BitConverter.ToString(page.ToArray()));

        var pagePointers = actualRootPage.GetBranchPagePointers().ToArray();
        var expectedPagePointers = expectedRootPage.GetBranchPagePointers().ToArray();

        Assert.Equal(expectedPagePointers, pagePointers);
    }

    [Fact]
    public void RootPageEncoderThrowsWhenPageIsTooSmall()
    {
        var rootPage = new RootPage(1, 0, 0, 0, 0, new byte[4096]);
        var systemUnderTest = new RootPageEncoder();
        var page = new Memory<byte>(new byte[512]);

        Assert.Throws<InvalidOperationException>(() => systemUnderTest.EncodePage(rootPage, page));
        rootPage = rootPage with {PageSizePower = 2};
        Assert.Throws<ArgumentException>(() => systemUnderTest.EncodePage(rootPage, page));

    }
}