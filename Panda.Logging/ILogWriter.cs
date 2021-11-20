namespace Panda.Logging;

public interface ILogWriter
{
    Task WriteAsync(Stream writer, LogEntry logEntry, CancellationToken cancellationToken);
}