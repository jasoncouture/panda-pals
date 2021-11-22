using System.Diagnostics;

namespace Panda.Logging.Physical.File;

public class LogAppender : ILogAppender, IAsyncDisposable
{
    private readonly ILogWriter _logWriter;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly DirectoryInfo _directory;
    private readonly long _segmentSize;
    private long _currentSerialNumber;
    private long _currentSegment = -1;
    private long _writesSinceLastFlush = 0;
    private Stream? _currentFile = null;
    private TaskCompletionSource _currentTaskCompletionSource = new();
    private bool _disposed;

    public LogAppender(string logDirectory, long startLogSerialNumber, long segmentCount, ILogWriter logWriter)
    {
        _logWriter = logWriter;
        _directory = Directory.CreateDirectory(logDirectory);
        _currentSerialNumber = startLogSerialNumber;
        _segmentSize = segmentCount;
    }

    private void ThrowIfDisposed()
    {
        if (!_disposed) return;
        throw new ObjectDisposedException(nameof(LogAppender));
    }

    private long ComputeSegment(long current) => (current / _segmentSize) * _segmentSize;
    public async Task<LogAppendResult> AppendAsync(byte[] data, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            List<Task> pendingTasks = new();
            var logEntrySequence = Interlocked.Increment(ref _currentSerialNumber);
            var segment = ComputeSegment(logEntrySequence);
            if (_currentFile == null || _currentSegment != segment)
            {
                var targetFileName = GetSegmentFilename(segment);
                // File switch time!
                _currentSegment = segment;
                var newFileStream = System.IO.File.Open(Path.Combine(_directory.FullName, targetFileName), FileMode.Append,
                    FileAccess.Write, FileShare.Read | FileShare.Delete);
                if (_currentFile != null) // If this isn't our first write, we'll need to clean up the last file.
                {
                    await FlushInternalAsync(cancellationToken).ConfigureAwait(false);
                    await _currentFile.DisposeAsync().ConfigureAwait(false);
                }

                _currentFile = newFileStream;

            }

            await _logWriter.WriteAsync(_currentFile, new LogEntry(logEntrySequence, data), cancellationToken).ConfigureAwait(false);
            var result = new LogAppendResult(logEntrySequence, _currentTaskCompletionSource.Task);

            var pendingWrites = Interlocked.Increment(ref _writesSinceLastFlush);
            if (pendingWrites <= 1000) return result;
            // This ensures that all these writes are flushed to disk.
            // And notifies threads that the log entries are now durable.
            await FlushInternalAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task FlushAsync(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            await FlushInternalAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task FlushInternalAsync(CancellationToken cancellationToken)
    {
        Debug.Assert(_currentFile != null, nameof(_currentFile) + " != null");
        await _currentFile.FlushAsync(cancellationToken).ConfigureAwait(false);
        Interlocked.Exchange(ref _writesSinceLastFlush, 0L);
        _currentTaskCompletionSource.TrySetResult();
        _currentTaskCompletionSource = new TaskCompletionSource();
    }

    private string GetSegmentFilename(long segment)
    {
        
        return $"{(segment):X16}.txl";
    }

    public async ValueTask DisposeAsync()
    {
        if (_currentFile == null) return;
        await _semaphore.WaitAsync(CancellationToken.None).ConfigureAwait(false);
        try
        {
            if (_currentFile == null) return;
            await FlushInternalAsync(CancellationToken.None).ConfigureAwait(false);
            await _currentFile.DisposeAsync().ConfigureAwait(false);
            _currentFile = null;
            _disposed = true;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}