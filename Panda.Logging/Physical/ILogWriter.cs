namespace Panda.Logging.Physical;

public interface ILogWriter
{
    Task WriteAsync(Stream writer, LogEntry logEntry, CancellationToken cancellationToken);
}