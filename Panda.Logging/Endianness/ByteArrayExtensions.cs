namespace Panda.Logging.Endianness;

public static class ByteArrayExtensions
{
    public static byte[] EnsureLittleEndian(this byte[] bytes)
    {
        if (BitConverter.IsLittleEndian) return bytes;
        return bytes.Reverse().ToArray();
    }
}