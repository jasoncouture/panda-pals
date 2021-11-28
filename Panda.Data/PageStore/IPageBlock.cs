namespace Panda.Data.PageStore;

public interface IPageBlock : IDisposable
{
    Memory<byte> Memory { get; }
    void MakeDurable();
}