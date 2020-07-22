using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class ChannelAudioSampler : AudioSampler
    {
        private readonly Speaker targetSpeaker;
        private readonly SampleChannelMapping[] channelMappings = new SampleChannelMapping[]
        {
            new MonoSampleChannelMapping(),
            new OnePointOneSampleChannelMapping(),
            new StereoSampleChannelMapping(),
            new TwoPointOneSampleChannelMapping(),
            new ThreePointZeroSampleChannelMapping(),
            new ThreePointOneSampleChannelMapping(),
            new QuadSampleChannelMapping(),
            new SurroundSampleChannelMapping(),
            new FivePointZeroSampleChannelMapping(),
            new FivePointOneSampleChannelMapping(),
            new SevenPointZeroSampleChannelMapping(),
            new SevenPointOneSampleChannelMapping(),
            new FivePointOneSurroundSampleChannelMapping(),
            new SevenPointOneSurroundSampleChannelMapping()
        };
        private readonly Dictionary<Speaker, Func<SampleChannelMapping, Func<byte[], ushort, byte[]>>> converterResolver = new Dictionary<Speaker, Func<SampleChannelMapping, Func<byte[], ushort, byte[]>>>()
        {
            { Speaker.Mono, x => x.ToMono },
            { Speaker.OnePointOne, x => x.ToOnePointOne },
            { Speaker.Stereo, x => x.ToStereo },
            { Speaker.TwoPointOne, x => x.ToTwoPointOne },
            { Speaker.ThreePointZero, x => x.ToThreePointZero },
            { Speaker.ThreePointOne, x => x.ToThreePointOne },
            { Speaker.Quad, x => x.ToQuad },
            { Speaker.Surround, x => x.ToSurround },
            { Speaker.FivePointZero, x => x.ToFivePointZero },
            { Speaker.FivePointOne, x => x.ToFivePointOne },
            { Speaker.SevenPointZero, x => x.ToSevenPointZero },
            { Speaker.SevenPointOne, x => x.ToSevenPointOne },
            { Speaker.FivePointOneSurround, x => x.ToFivePointOneSurround },
            { Speaker.SevenPointOneSurround, x => x.ToSevenPointOneSurround }
        };

        public ChannelAudioSampler(Speaker targetSpeaker)
        {
            //TODO: get wasapi channel mapping
            this.targetSpeaker = targetSpeaker;
        }

        public ChannelAudioSampler(ushort targetChannels)
        {
            this.targetSpeaker = ChannelFactory.GetDefaultMapping(targetChannels);
        }

        [return: NotNull]
        public override Task<WaveEnumerableAudioContainer> SampleAsync([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            // TODO: check if channel config is allready matching
            var targetChannels = ChannelFactory.GetChannels(targetSpeaker);
            var factor = targetChannels / (double) audio.FmtSubChunk.Channels;

            return Task.FromResult(audio
                    .WithFmtSubChunk(x => x
                        .WithChannels((ushort)targetChannels))
                    .WithDataSubChunk(x => x
                        .WithChunkSize((uint) (factor * audio.DataSubChunk.ChunkSize))
                        .WithData(ManipulateAudioData(audio, cancellationToken))));
        }

        [return: NotNull]
        private async IAsyncEnumerable<byte[]> ManipulateAudioData([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull] [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            //TODO: better way to get source channels
            Speaker sourceSpeaker = ChannelFactory.GetDefaultMapping(audio.FmtSubChunk.Channels);
            var channelMapping = channelMappings.First(x => x.Speaker == sourceSpeaker);
            var samples = audio.DataSubChunk.Data;
            var converter = converterResolver[targetSpeaker].Invoke(channelMapping);
            var temp = new byte[audio.FmtSubChunk.Channels * audio.FmtSubChunk.BitsPerSample / 8];
            var targetChannels = ChannelFactory.GetChannels(targetSpeaker);

            var counter = 0;
            await foreach (var value in samples.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                Array.Copy(value, 0, temp, counter * audio.FmtSubChunk.BitsPerSample / 8, value.Length);
                if (++counter == audio.FmtSubChunk.Channels)
                {
                    var convertedSample = converter.Invoke(temp, audio.FmtSubChunk.BitsPerSample);

                    for (int i = 0; i < targetChannels; i++)
                    {
                        yield return convertedSample.Skip(i * audio.FmtSubChunk.BitsPerSample / 8).Take(audio.FmtSubChunk.BitsPerSample / 8).ToArray();
                    }
                    counter = 0;
                }
            }
        }
    }
}
