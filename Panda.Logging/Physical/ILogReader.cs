namespace Panda.Logging.Physical;

public interface ILogReader
{
    Task<LogEntry> ReadAsync(Stream reader, CancellationToken cancellationToken) =>
        ReadAsync(reader, validateChecksum: true, cancellationToken);
    Task<LogEntry> ReadAsync(Stream reader, bool validateChecksum, CancellationToken cancellationToken);
}