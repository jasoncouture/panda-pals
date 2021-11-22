namespace Panda.Logging.Physical;

public record LogEntry(long SerialNumber, long Timestamp, byte[] Data)
{
    public LogEntry(long serialNumber, DateTimeOffset timestamp, byte[] data) : this(serialNumber, timestamp.ToUnixTimeMilliseconds(), data) { }
    public LogEntry(long serialNumber, byte[] data) : this(serialNumber, DateTimeOffset.Now, data) { }
};