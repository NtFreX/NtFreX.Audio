using System;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    public interface ISeekableAsyncEnumerator<T> : IAsyncDisposable
    {
        //TODO: remove all depencencies for c# lang < v9 (dotnet core 3.1 and 5.0?) and make this nullable
        T Current { get; }

        /// <summary>
        /// The size/length of the collection which is enumerated
        /// </summary>
        /// <returns>
        /// Throws NotSupportedException if the collection is endless.
        /// Else the size/length of the collection</returns>
        ulong GetDataLength();
        bool CanGetLength();

        /// <summary>
        /// The position at start is -1.
        /// After MoveNextAsync has been called the first time it will be 0 and the value of Current will be set to the first element int the collection.
        /// </summary>
        /// <returns>
        /// If the collection is endless null.
        /// Else the current position in the collection.
        /// </returns>
        long GetPosition();

        void SeekTo(long position);
        bool CanSeek();

        ValueTask<bool> MoveNextAsync();
    }
}
