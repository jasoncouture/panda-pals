namespace Panda.Data.PageStore;

public interface IReadOnlyPageBlock : IDisposable
{
    ReadOnlyMemory<byte> ReadOnlyMemory { get; }
}