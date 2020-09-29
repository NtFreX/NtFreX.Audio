using System;
using System.IO;

namespace NtFreX.Audio.Infrastructure.Threading.Extensions
{
    public static class StreamExtensions
    {
        public static StreamEnumerable ToEnumerable(this Stream stream)
            => stream.ToEnumerable(0);

        public static StreamEnumerable ToEnumerable(this Stream stream, long startIndex)
            => stream.ToEnumerable(startIndex, stream?.Length ?? throw new ArgumentNullException(nameof(stream)));

        public static StreamEnumerable ToEnumerable(this Stream stream, long startIndex, long endIndex)
            => new StreamEnumerable(stream, startIndex, endIndex);
    }
}
