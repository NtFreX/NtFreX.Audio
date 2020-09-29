namespace NtFreX.Audio.Infrastructure.Threading.Extensions
{
    public static class ArrayExtensions
    {
        public static ISeekableAsyncEnumerable<T> ToSeekableAsyncEnumerable<T>(this T[] array)
            => new SeekableArrayEnumerable<T>(array);
    }
}
