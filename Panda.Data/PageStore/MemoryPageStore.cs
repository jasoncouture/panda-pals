using System.Buffers;
using System.Collections.Concurrent;

namespace Panda.Data.PageStore;

public class MemoryPageStore : IPageStore
{
    private readonly ArrayPool<byte> _pagePool;
    private const int MaxPageSize = 1024 * 1024 * 64;
    public int PageSize { get; } = 512;
    public MemoryPageStore()
    {
        _pagePool = ArrayPool<byte>.Create(MaxPageSize, 256);
    }
    private readonly ConcurrentDictionary<ulong, byte[]> _pages = new();
    public IReadOnlyPageBlock OpenReadOnlyPage(ulong pageNumber)
    {
        var buffer = GetPageBuffer(pageNumber);

        return new ReadOnlyPageBlock(buffer, null);
    }

    private byte[] GetPageBuffer(ulong pageNumber)
    {
        while (!_pages.ContainsKey(pageNumber))
        {
            var rentedPool = _pagePool.Rent(PageSize);
            if (!_pages.TryAdd(pageNumber, rentedPool))
                _pagePool.Return(rentedPool);
        }

        return _pages[pageNumber];
    }

    public IPageBlock OpenPage(ulong pageNumber)
    {
        var buffer = GetPageBuffer(pageNumber);
        return new WritablePageBlock(buffer, null, pageNumber, this);
    }

    public void WritePage(byte[] buffer, ulong pageNumber)
    {
        // Do nothing, since we gave the page the actual buffer.
    }
}