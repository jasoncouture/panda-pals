using System;
using Panda.Data.Pages.Data;
using Xunit;

namespace Panda.Data.UnitTests;

public class DataPageTests
{
    [Fact]
    public void CanDecodeEncodedDataPage()
    {
        var data = Guid.NewGuid().ToByteArray();
        var expectedPage = new DataPage(1ul, 2u, data);
        var dataPageEncoder = new DataPageEncoder();
        var dataPageDecoder = new DataPageDecoder();
        var buffer = new Memory<byte>(new byte[512]);
        dataPageEncoder.EncodePage(expectedPage, buffer);
        var actualPage = dataPageDecoder.DecodePage(buffer);

        Assert.Equal(expectedPage.NextDataPage, actualPage.NextDataPage);
        Assert.Equal(expectedPage.Checksum, actualPage.Checksum);
        Assert.Equal(expectedPage.Data, actualPage.Data);
    }
}