namespace Panda.Logging.Physical;

public interface IChecksumProvider
{
    uint ComputeChecksum(params IEnumerable<byte>[] bytes);
    bool VerifyChecksum(uint checkSum, params IEnumerable<byte>[] bytes) => ComputeChecksum(bytes) == checkSum;
}