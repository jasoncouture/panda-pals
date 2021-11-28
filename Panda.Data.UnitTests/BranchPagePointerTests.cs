using System;
using System.Linq;
using Panda.Data.Pages.Root;
using Xunit;

namespace Panda.Data.UnitTests;

public class BranchPagePointerTests
{
    [Fact]
    public void BranchPagePointerEncodeDecodeTest()
    {
        var expectedBranchPagePointer = new BranchPagePointer(
            Guid.Empty,
            new Guid(Enumerable.Range(0, 16).Select(_ => (byte) 0xFF).ToArray()),
            1ul
        );
        const bool expectedResult = true;
        // Single bucket, zero Guid to max guid

        var memory = new Memory<byte>(new byte[BranchPagePointer.Size]);

        expectedBranchPagePointer.WriteTo(memory.Span);

        var result = BranchPagePointer.TryParseFromSpan(memory.Span, out var actualBranchPagePointer);
        Assert.Equal(expectedResult, result);
        Assert.Equal(expectedBranchPagePointer, actualBranchPagePointer);
    }

}