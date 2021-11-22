using System.Runtime.CompilerServices;

namespace Panda.Logging.Physical.File;

public record LogAppendResult(long SequenceNumber, Task Finalized)
{
    public TaskAwaiter GetAwaiter()
    {
        return Finalized.GetAwaiter();
    }

    public static implicit operator long(LogAppendResult logAppendResult)
    {
        return logAppendResult.SequenceNumber;
    }
}