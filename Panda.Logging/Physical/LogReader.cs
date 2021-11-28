using Panda.DataIntegrity;
using Panda.Logging.Endianness;
using System.Buffers;

namespace Panda.Logging.Physical;

public class LogReader : ILogReader
{
    private readonly IChecksumProvider _checksumProvider;
    const int BufferSize = 16384;
    private static ArrayPool<byte> LongPool = ArrayPool<byte>.Create(sizeof(long), 16384);
    private static ArrayPool<byte> IntPool = ArrayPool<byte>.Create(sizeof(int), 16384);
    private static ArrayPool<byte> BufferPool = ArrayPool<byte>.Create(BufferSize, 16384);
    public LogReader(IChecksumProvider checksumProvider)
    {
        _checksumProvider = checksumProvider;
    }

    public async Task<LogEntry> ReadAsync(Stream reader, bool validateChecksum, CancellationToken cancellationToken)
    {
        const int SequenceByteLength = sizeof(long);
        const int TimestampByteLength = sizeof(long);
        const int LengthByteLength = sizeof(long);
        const int ChecksumByteLength = sizeof(uint);
        
        // This function will be used _a lot_
        // So, let's avoid any non-stack allocations we can where possible.
        // ArrayPool will allow us to share these buffers and never free them.
        // but we can't use it for the actual data bytes, and we MUST make sure they
        // get returned at the end of the function.
        var sequenceBytes = LongPool.Rent(SequenceByteLength);
        var timestampBytes = LongPool.Rent(TimestampByteLength);
        var lengthBytes = LongPool.Rent(LengthByteLength);

        var checksumBytes = IntPool.Rent(ChecksumByteLength);

        var readBuffer = BufferPool.Rent(BufferSize);
        try
        {
            var versionByte = reader.ReadByte();
            if (versionByte > LogFormatConstants.LogVersion)
                throw new InvalidOperationException("Unexpected log version!")
                {
                    Data =
                    {
                        ["versionFound"] = versionByte,
                        ["maximumSupportedVersion"] = LogFormatConstants.LogVersion
                    }
                };
            await reader.ReadAsync(sequenceBytes, 0, SequenceByteLength, cancellationToken).ConfigureAwait(false);
            await reader.ReadAsync(timestampBytes, 0, TimestampByteLength, cancellationToken).ConfigureAwait(false);
            await reader.ReadAsync(lengthBytes, 0, LengthByteLength, cancellationToken).ConfigureAwait(false);
            var dataByteLength = BitConverter.ToInt64(lengthBytes.EnsureLittleEndian(), 0);
            var dataBytes = new byte[dataByteLength];
            var currentOffset = 0;

            while (dataByteLength > 0)
            {
                var bytesToRead = dataByteLength > readBuffer.Length ? readBuffer.Length : (int)dataByteLength;
                var bytesRead = await reader.ReadAsync(readBuffer, 0, bytesToRead, cancellationToken).ConfigureAwait(false);
                if (bytesRead == 0) throw new EndOfStreamException("The log stream ended unexpectedly.");
                Array.Copy(readBuffer, 0, dataBytes, currentOffset, bytesRead);
                currentOffset += bytesRead;
                dataByteLength -= bytesRead;

            }

            await reader.ReadAsync(checksumBytes, 0, ChecksumByteLength, cancellationToken).ConfigureAwait(false);

            if (validateChecksum)
            {
                var checksum = BitConverter.ToUInt32(checksumBytes.EnsureLittleEndian());
                if (!_checksumProvider.VerifyChecksum(checksum, sequenceBytes.EnsureLittleEndian(), timestampBytes.EnsureLittleEndian(), lengthBytes.EnsureLittleEndian(), dataBytes))
                {
                    throw new InvalidDataException("Checksum in stream does not match computed checksum.");
                }
            }

            var serialNumber = BitConverter.ToInt64(sequenceBytes.EnsureLittleEndian());
            var timestamp = BitConverter.ToInt64(timestampBytes.EnsureLittleEndian());

            return new LogEntry(serialNumber, timestamp, dataBytes);
        }
        finally
        {
            LongPool.Return(sequenceBytes);
            LongPool.Return(timestampBytes);
            LongPool.Return(lengthBytes);
            BufferPool.Return(readBuffer);
            IntPool.Return(checksumBytes);
        }
    }
}