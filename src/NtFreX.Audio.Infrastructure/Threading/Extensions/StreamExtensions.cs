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

        /// <summary>
        /// This code exists to not depend on the CanSeek property of the stream to determinate if the length can be read.
        /// This assumes CanSeek and CanGetLength are two different concepts.
        /// </summary>
        /// <param name="stream">The stream where the length is read</param>
        /// <param name="length">If the length can be read this variable will be set</param>
        /// <returns>If the length of the stream can be read</returns>
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
