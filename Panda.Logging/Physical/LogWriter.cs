using Panda.DataIntegrity;
using Panda.Logging.Endianness;

namespace Panda.Logging.Physical;

public class LogWriter : ILogWriter
{
    private readonly IChecksumProvider _checksumProvider;

    public LogWriter(IChecksumProvider checksumProvider)
    {
        _checksumProvider = checksumProvider;
    }

    public async Task WriteAsync(Stream writer, LogEntry logEntry, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested(); // Last chance, otherwise we're doing this and ignoring your cancellationToken...
        var serialNumberBytes = BitConverter.GetBytes(logEntry.SerialNumber).EnsureLittleEndian();
        var timestampBytes = BitConverter.GetBytes(logEntry.Timestamp).EnsureLittleEndian();
        var dataLengthBytes = BitConverter.GetBytes(logEntry.Data.LongLength).EnsureLittleEndian();
        // This does mean we loop over the data twice, but we do avoid copying all of it to a new buffer
        // to compute the checksum.
        var checksum = _checksumProvider.ComputeChecksum(LogFormatConstants.LogVersionBytes, serialNumberBytes, timestampBytes, dataLengthBytes, logEntry.Data);
        var checksumBytes = BitConverter.GetBytes(checksum).EnsureLittleEndian();
        await WriteWithoutCancellation(writer, LogFormatConstants.LogVersionBytes, serialNumberBytes, timestampBytes, dataLengthBytes, logEntry.Data,
            checksumBytes).ConfigureAwait(false);
    }

    private async Task WriteWithoutCancellation(Stream writer, params byte[][] dataBlocks)
    {
        foreach (var block in dataBlocks)
        {
            if (block.Length == 0) continue;
            await writer.WriteAsync(block, CancellationToken.None).ConfigureAwait(false);
        }
    }
}