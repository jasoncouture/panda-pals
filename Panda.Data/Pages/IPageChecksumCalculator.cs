namespace Panda.Data.Pages;

public interface IPageChecksumCalculator<in T> where T : IPage<T>
{
    uint ComputeChecksum(T page);
}