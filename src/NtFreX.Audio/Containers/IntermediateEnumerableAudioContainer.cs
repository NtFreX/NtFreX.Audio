using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers
{
    public sealed class IntermediateEnumerableAudioContainer : IntermediateAudioContainer
    {
        private readonly ISeekableAsyncEnumerable<Sample> data;

        public IntermediateEnumerableAudioContainer(ISeekableAsyncEnumerable<Sample> data, IAudioFormat format, bool isLittleEndian)
            : base(format, isLittleEndian)
        {
            this.data = data;
        }

        public override ulong GetDataLength()
            => data.GetDataLength();
        public override bool CanGetDataLength()
            => data.CanGetLength();

        public override ValueTask DisposeAsync()
            => data.DisposeAsync();

        public override void Dispose() { }

        public IntermediateEnumerableAudioContainer WithData(ISeekableAsyncEnumerable<Sample> data, IAudioFormat? format = null, bool? isDataLittleEndian = null)
            => new IntermediateEnumerableAudioContainer(data, format ?? GetFormat(), isDataLittleEndian ?? IsDataLittleEndian());

        public override ISeekableAsyncEnumerator<Sample> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => data.GetAsyncEnumerator(cancellationToken);
    }
}