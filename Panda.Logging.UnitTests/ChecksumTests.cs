using System;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace Panda.Logging.UnitTests;

public class ChecksumTests
{
    [Theory()]
    [InlineData("", 0u)]
    [InlineData("123456789", 0xCBF43926u)]
    // Source: https://rosettacode.org/wiki/CRC-32
    [InlineData("The quick brown fox jumps over the lazy dog", 0x414FA339u)]
    // Source: http://cryptomanager.com/tv.html
    [InlineData("various CRC algorithms input data", 0x9BD366AE)]
    // Source: http://www.febooti.com/products/filetweak/members/hash-and-crc/test-vectors/
    [InlineData("Test vector from febooti.com", 0x0C877F61)]
    public void Crc32KnownValueTestCases(string inputString, uint expected)
    {
        var sut = new Crc32CheckSumProvider();
        var inputData = Encoding.UTF8.GetBytes(inputString);


        var actual = sut.ComputeChecksum(inputData);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("", 0u, true)]
    [InlineData("", 1u, false)]
    [InlineData("123456789", 0xCBF43926u, true)]
    [InlineData("123456789", 0xCBF43927u, false)]
    // Source: https://rosettacode.org/wiki/CRC-32
    [InlineData("The quick brown fox jumps over the lazy dog", 0x414FA339u, true)]
    [InlineData("The quick brown fox jumps over the lazy dog", 0x414FA330u, false)]
    // Source: http://cryptomanager.com/tv.html
    [InlineData("various CRC algorithms input data", 0x9BD366AE, true)]
    [InlineData("various CRC algorithms input data", 0x9BD366AF, false)]
    // Source: http://www.febooti.com/products/filetweak/members/hash-and-crc/test-vectors/
    [InlineData("Test vector from febooti.com", 0x0C877F61, true)]
    [InlineData("Test vector from febooti.com", 0x1C877F61, false)]
    public void VerifyChecksumMatchesExpectedBehavior(string inputString, uint checksum, bool expected = true)
    {
        IChecksumProvider sut = new Crc32CheckSumProvider();
        var inputBytes = Encoding.UTF8.GetBytes(inputString);
        var result = sut.VerifyChecksum(inputBytes, checksum);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Crc32NullInputResultsInNullReferenceException()
    {
        var sut = new Crc32CheckSumProvider();

        Assert.Throws<ArgumentNullException>("byteStream", () => sut.ComputeChecksum(null!));
    }
}