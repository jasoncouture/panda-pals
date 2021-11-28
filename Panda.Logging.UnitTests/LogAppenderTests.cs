using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Panda.DataIntegrity;
using Panda.Logging.Physical;
using Panda.Logging.Physical.File;
using Xunit;

namespace Panda.Logging.UnitTests;

public class LogAppenderTests
{
    [Fact]
    public async Task LogAppenderCreatesFilesInTargetFolder()
    {
        const long entriesPerSegment = 5000;
        const int dataSize = sizeof(long);
        // Log format is: Serial (64 bit) - Data size (64 bit) - Data (Data size * 8 bits) - Checksum (32 bits)
        const int recordSize = sizeof(byte) + sizeof(long) + sizeof(long)  + sizeof(long) + dataSize + sizeof(int);
        var expectedFileSize = recordSize * entriesPerSegment;
        var target = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), nameof(LogAppenderTests), Path.GetRandomFileName()));
        await using var logAppender = new LogAppender(target.FullName, -1, entriesPerSegment, new LogWriter(new Crc32CheckSumProvider()));
        for (long x = 0; x < entriesPerSegment; x++)
        {
            var result =  await logAppender.AppendAsync(BitConverter.GetBytes(x), CancellationToken.None);
            Assert.Equal(x, result);
        }

        var files = target.GetFiles("*.txl");
        var firstFile = Assert.Single(files);


        await logAppender.AppendAsync(BitConverter.GetBytes(ulong.MaxValue), CancellationToken.None); // This append will start at the start of the next segment file.
        // When this happens, the original file should be flushed and closed.
        firstFile.Refresh();
        var fileLength = firstFile.Length;
        Assert.Equal(expectedFileSize, fileLength);
        firstFile.Delete();
        
        files = target.GetFiles("*.txl");
        var secondFile = Assert.Single(files);
        await logAppender.FlushAsync(CancellationToken.None);
        secondFile.Refresh();
        Assert.Equal(recordSize, secondFile.Length);

        

        foreach (var file in target.GetFiles("*.txl"))
        {
            file.Delete();
        }
    }
}