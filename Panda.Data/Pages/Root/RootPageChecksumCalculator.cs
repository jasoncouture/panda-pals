using Panda.DataIntegrity;

namespace Panda.Data.Pages.Root;

public class RootPageChecksumCalculator : IPageChecksumCalculator<RootPage>
{
    private readonly IChecksumProvider _checksumProvider;

    public RootPageChecksumCalculator(IChecksumProvider checksumProvider)
    {
        _checksumProvider = checksumProvider;
    }

    public uint ComputeChecksum(RootPage page)
    {
        var checksumParts = new byte[][]
        {
            new[] {page.FileVersion, page.PageSizePower},
            BitConverter.GetBytes(page.PageData?.Length ?? 0),
            BitConverter.GetBytes(page.RootContinuationPageNumber),
            BitConverter.GetBytes(page.FreePageIndexPageNumber),
            page.PageData ?? Array.Empty<byte>()
        }.Select(i => i.AsEnumerable()).ToArray();

        return _checksumProvider.ComputeChecksum(checksumParts);
    }
}