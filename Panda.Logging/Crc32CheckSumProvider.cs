namespace Panda.Logging;

/// <summary>
/// Performs 32-bit reversed cyclic redundancy checks.
/// </summary>
public class Crc32CheckSumProvider : IChecksumProvider
{
    private const uint Generator = 0xEDB88320;
    private readonly uint[] _checksumTable;

    public Crc32CheckSumProvider()
    {
        // Constructs the checksum lookup table. Used to optimize the checksum.
        _checksumTable = Enumerable.Range(0, 256).Select(i =>
        {
            var tableEntry = (uint)i;
            for (var j = 0; j < 8; ++j)
            {
                tableEntry = ((tableEntry & 1) != 0)
                    ? (Generator ^ (tableEntry >> 1))
                    : (tableEntry >> 1);
            }
            return tableEntry;
        }).ToArray();
    }

    public uint ComputeChecksum(IEnumerable<byte> byteStream)
    {
        if (byteStream == null) throw new ArgumentNullException(nameof(byteStream));
        // Initialize checksumRegister to 0xFFFFFFFF and calculate the checksum.
        return ~byteStream.Aggregate(0xFFFFFFFF, (checksumRegister, currentByte) =>
            (_checksumTable[(checksumRegister & 0xFF) ^ Convert.ToByte(currentByte)] ^ (checksumRegister >> 8)));
    }
}