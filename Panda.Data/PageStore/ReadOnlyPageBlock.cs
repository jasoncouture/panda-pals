using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Panda.Data.Pages;

namespace Panda.Data.PageStore;

public class ReadOnlyPageBlock : IReadOnlyPageBlock
{
    private byte[]? _buffer;
    private readonly ArrayPool<byte>? _bufferPool;

    public ReadOnlyPageBlock(byte[] buffer, ArrayPool<byte>? bufferPool)
    {
        _buffer = buffer;
        ReadOnlyMemory = new ReadOnlyMemory<byte>(buffer);
        _bufferPool = bufferPool;
    }
    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (disposing) GC.SuppressFinalize(this);
        var buffer = _buffer;
        _buffer = null;
        ReadOnlyMemory = null;
        if (buffer == null) return;
        _bufferPool?.Return(buffer);
    }

    ~ReadOnlyPageBlock()
    {
        Dispose(false);
    }

    public ReadOnlyMemory<byte> ReadOnlyMemory { get; private set; }
}