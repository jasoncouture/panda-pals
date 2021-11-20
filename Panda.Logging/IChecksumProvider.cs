namespace Panda.Logging;

public interface IChecksumProvider
{
    uint ComputeChecksum(IEnumerable<byte> bytes);
    bool VerifyChecksum(IEnumerable<byte> bytes, uint checkSum) => ComputeChecksum(bytes) == checkSum;
}