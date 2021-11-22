using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Panda.Logging.Physical;
using Xunit;

namespace Panda.Logging.UnitTests;

public class LogReaderWriterTests
{

    private readonly IChecksumProvider _checksumProvider;
    private readonly Mock<IChecksumProvider> _mockChecksumProvider;
    public LogReaderWriterTests()
    {
        var checksumProviderMock = new Mock<IChecksumProvider>();
        checksumProviderMock
            .Setup(i => i.VerifyChecksum(It.IsAny<uint>(), It.IsAny<IEnumerable<byte>[]>()))
            .Returns(true);

        checksumProviderMock
            .Setup(i => i.ComputeChecksum(It.IsAny<IEnumerable<byte>[]>()))
            .Returns(0u);
        _checksumProvider = checksumProviderMock.Object;
        _mockChecksumProvider = checksumProviderMock;
    }
    [Fact]
    public async Task LogWriterBasicVerificationTests()
    {
        var logWriter = new LogWriter(_checksumProvider);
        // Because we are writing no data, we expect this record to be 28 bytes.
        const int expectedLength = sizeof(byte) + sizeof(long) + sizeof(long) + sizeof(long) + sizeof(int);
        const byte expectedByte = 0; // We expect all bytes to be 0, because we have a 0 serial number, 0 length, and 0 checksum (see constructor)
        await using var memoryStream = new MemoryStream();
        await logWriter.WriteAsync(memoryStream, new LogEntry(0, 0, Array.Empty<byte>()), CancellationToken.None);
        var bytes = memoryStream.ToArray();

        Assert.Equal(expectedLength, bytes.Length);
        Assert.All(bytes, b => Assert.Equal(expectedByte, b));
    }

    [Theory]
    [MemberData(nameof(LogEntries))]
    public async Task LogReaderCanDecodeOutputOfLogWriter(LogEntry sourceLogEntry)
    {
        var logWriter = new LogWriter(_checksumProvider);
        var logReader = new LogReader(_checksumProvider);

        await using var memoryStream = new MemoryStream();
        await logWriter.WriteAsync(memoryStream, sourceLogEntry, CancellationToken.None);
        await memoryStream.FlushAsync(CancellationToken.None);
        memoryStream.Seek(0, SeekOrigin.Begin);
        var logEntry = await logReader.ReadAsync(memoryStream, true, CancellationToken.None);

        Assert.NotNull(logEntry);
        Assert.Equal(sourceLogEntry.SerialNumber, logEntry.SerialNumber);
        Assert.Equal(sourceLogEntry.Data, logEntry.Data);
    }

    public static IEnumerable<object[]> LogEntries()
    {
        yield return new object[] { new LogEntry(1, Array.Empty<byte>()) };
        var largeData = new byte[16384 * 4];
        yield return new object[] { new LogEntry(long.MinValue, largeData) };
    }
}