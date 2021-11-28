using System.Buffers;

namespace Panda.Data.PageStore;

public class WritablePageBlock : IPageBlock
{
    private byte[]? _buffer;
    private readonly ArrayPool<byte>? _bufferPool;
    private readonly IPageStore _pageStore;

    public WritablePageBlock(byte[] buffer, ArrayPool<byte>? bufferPool, ulong pageNumber, IPageStore pageStore)
    {
        _buffer = buffer;
        Memory = new Memory<byte>(buffer);
        _bufferPool = bufferPool;
        PageNumber = pageNumber;
        _pageStore = pageStore;
    }

    public ulong PageNumber { get; set; }

    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (disposing) GC.SuppressFinalize(this);
        if (_buffer == null) return;
        _bufferPool?.Return(_buffer);
        _buffer = null;
        Memory = null;
    }

    ~WritablePageBlock()
    {
        Dispose(false);
    }

    public Memory<byte> Memory { get; private set; }
    public void MakeDurable()
    {
        if (_buffer == null) throw new ObjectDisposedException("This page has been disposed and cannot be made durable.");
        _pageStore.WritePage(_buffer, PageNumber);
    }
}