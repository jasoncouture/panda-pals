namespace Panda.Data.Pages.Free;

public class FreePageChecksumCalculator : IPageChecksumCalculator<FreePage>
{
    // Free pages don't use checksum :)
    public uint ComputeChecksum(FreePage page) => 0u;
}