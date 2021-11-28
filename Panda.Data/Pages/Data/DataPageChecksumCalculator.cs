using Panda.DataIntegrity;

namespace Panda.Data.Pages.Data;

public class DataPageChecksumCalculator : IPageChecksumCalculator<DataPage>
{
    private readonly IChecksumProvider _checksumProvider;

    public DataPageChecksumCalculator(IChecksumProvider checksumProvider)
    {
        _checksumProvider = checksumProvider;
    }

    public uint ComputeChecksum(DataPage page) =>
        _checksumProvider.ComputeChecksum(
            BitConverter.GetBytes(page.NextDataPage),
            BitConverter.GetBytes(page.Data.Length), 
            page.Data
        );
}