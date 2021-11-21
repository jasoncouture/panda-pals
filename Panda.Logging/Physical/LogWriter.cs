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
        var serialNumberBytes = BitConverter.GetBytes(logEntry.SerialNumber).EnsureLittleEndian();
        var dataLengthBytes = BitConverter.GetBytes(logEntry.Data.LongLength).EnsureLittleEndian();
        // This does mean we loop over the data twice, but we do avoid copying all of it to a new buffer
        // to compute the checksum.
        var checksum = _checksumProvider.ComputeChecksum(serialNumberBytes, dataLengthBytes, logEntry.Data);
        await writer.WriteAsync(serialNumberBytes, cancellationToken).ConfigureAwait(false);
        await writer.WriteAsync(dataLengthBytes, cancellationToken).ConfigureAwait(false);

        if (logEntry.Data.LongLength > 0)
            await writer.WriteAsync(logEntry.Data, cancellationToken).ConfigureAwait(false);
        await writer.WriteAsync(BitConverter.GetBytes(checksum).EnsureLittleEndian(), cancellationToken).ConfigureAwait(false);
    }
}