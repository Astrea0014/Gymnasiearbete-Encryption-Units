using System.IO.Pipes;

namespace Unit_X_Common
{
    public static class RuntimeHelper
    {
        public static byte[] ReadPipe(PipeStream pipe)
        {
            byte[] payload = new byte[4096];
            Array.Resize(ref payload, pipe.Read(payload));
            return payload;
        }

        public static string FormatArray(byte[] data)
            => $"[{string.Join(", ", data.Select(o => $"0x{o:X2}"))}]";
    }
}
