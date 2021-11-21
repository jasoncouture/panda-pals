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

    public uint ComputeChecksum(params IEnumerable<byte>[] byteStream)
    {
        if (byteStream == null) throw new ArgumentNullException(nameof(byteStream));
        // Initialize checksumRegister to 0xFFFFFFFF and calculate the checksum.
        var register = 0xFFFFFFFF;
        for (var x = 0; x < byteStream.Length; x++)
        {
            register = ComputePartialChecksum(register, byteStream[x]);
        }
        return ~register;
    }

    private uint ComputePartialChecksum(uint register, IEnumerable<byte> bytes)
    {
        foreach (var b in bytes)
        {
            register = (_checksumTable[(register & 0xFF) ^ b] ^ (register >> 8));
        }

        return register;
    }
}