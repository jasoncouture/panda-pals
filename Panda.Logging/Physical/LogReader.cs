using Panda.Logging.Endianness;
using System.Buffers;

namespace Panda.Logging.Physical;

public class LogReader : ILogReader
{
    private readonly IChecksumProvider _checksumProvider;

    public LogReader(IChecksumProvider checksumProvider)
    {
        _checksumProvider = checksumProvider;
    }

    public async Task<LogEntry> ReadAsync(Stream reader, bool validateChecksum, CancellationToken cancellationToken)
    {
        const int SequenceByteLength = 8;
        const int LengthByteLength = 8;
        const int ChecksumByteLength = 4;
        const int BufferSize = 16384;
        // This function will be used _a lot_
        // So, let's avoid any non-stack allocations we can where possible.
        // ArrayPool will allow us to share these buffers and never free them.
        // but we can't use it for the actual data bytes, and we MUST make sure they
        // get returned at the end of the function.
        var sequenceBytes = ArrayPool<byte>.Shared.Rent(SequenceByteLength);
        var lengthBytes = ArrayPool<byte>.Shared.Rent(LengthByteLength);
        var checksumBytes = ArrayPool<byte>.Shared.Rent(ChecksumByteLength);
        var readBuffer = ArrayPool<byte>.Shared.Rent(BufferSize);
        try
        {
            await reader.ReadAsync(sequenceBytes, 0, SequenceByteLength, cancellationToken);
            await reader.ReadAsync(lengthBytes, 0, LengthByteLength, cancellationToken);
            var dataByteLength = BitConverter.ToInt64(lengthBytes.EnsureLittleEndian(), 0);
            var dataBytes = new byte[dataByteLength];
            int currentOffset = 0;

            while (dataByteLength > 0)
            {
                var bytesToRead = dataByteLength > readBuffer.Length ? readBuffer.Length : (int)dataByteLength;
                var bytesRead = await reader.ReadAsync(readBuffer, 0, bytesToRead, cancellationToken);
                if (bytesRead == 0) throw new EndOfStreamException("The log stream ended unexpectedly.");
                Array.Copy(readBuffer, 0, dataBytes, currentOffset, bytesRead);
                currentOffset += bytesRead;
                dataByteLength -= bytesRead;

            }

            await reader.ReadAsync(checksumBytes, 0, ChecksumByteLength, cancellationToken);

            if (validateChecksum)
            {
                var checksum = BitConverter.ToUInt32(checksumBytes.EnsureLittleEndian());
                if (!_checksumProvider.VerifyChecksum(checksum, sequenceBytes.EnsureLittleEndian(), lengthBytes.EnsureLittleEndian(), dataBytes))
                {
                    throw new InvalidDataException("Checksum in stream does not match computed checksum.");
                }
            }

            var serialNumber = BitConverter.ToInt64(sequenceBytes.EnsureLittleEndian());

            return new LogEntry(serialNumber, dataBytes);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(sequenceBytes);
            ArrayPool<byte>.Shared.Return(lengthBytes);
            ArrayPool<byte>.Shared.Return(readBuffer);
            ArrayPool<byte>.Shared.Return(checksumBytes);
        }
    }
}