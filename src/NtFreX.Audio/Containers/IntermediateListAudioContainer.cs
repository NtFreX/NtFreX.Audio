using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers
{
    public sealed class IntermediateListAudioContainer : IntermediateAudioContainer
    {
        private readonly Sample[] samples;

        public IntermediateListAudioContainer(Sample[] samples, IAudioFormat format, bool isLittleEndian)
            : base(format, isLittleEndian)
        {
            this.samples = samples;
        }

        public override long GetDataLength()
            => samples.Length;
        public override TimeSpan GetLength()
        {
            var format = GetFormat();
            return TimeSpan.FromSeconds(GetDataLength() / format.SampleRate / format.Channels);
        }

        public override ValueTask DisposeAsync()
            => new ValueTask(Task.CompletedTask);
        public override void Dispose() { }

        public override ISeekableAsyncEnumerator<Sample> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => samples.ToSeekableAsyncEnumerable().GetAsyncEnumerator(cancellationToken);

        public IntermediateEnumerableAudioContainer AsEnumerable()
            => new IntermediateEnumerableAudioContainer(this, GetFormat(), IsDataLittleEndian());
    }
}
