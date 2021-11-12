using System;
using System.IO;

namespace NtFreX.Audio.Infrastructure.Threading.Extensions
{
    public static class StreamExtensions
    {
        public static StreamEnumerable ToEnumerable(this Stream stream)
            => stream.ToEnumerable(0);

        public static StreamEnumerable ToEnumerable(this Stream stream, long startIndex)
            => stream.ToEnumerable(startIndex, stream.TryGetLength(out var length) ? (long?)length : null);

        public static StreamEnumerable ToEnumerable(this Stream stream, long startIndex, long? endIndex)
            => new StreamEnumerable(stream, startIndex, endIndex);

        public static bool TryGetLength(this Stream stream, out long length)
        {
            try
            {
                length = stream?.Length ?? -1;
                if (length >= 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                length = default;
                return false;
            }
        }
    }
}
