namespace Panda.Logging.Physical.File;

public interface ILogAppender
{
    Task<LogAppendResult> AppendAsync(byte[] data, CancellationToken cancellationToken);
    Task FlushAsync(CancellationToken cancellationToken);
}