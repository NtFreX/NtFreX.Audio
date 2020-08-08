using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class ChannelAudioSampler : AudioSampler
    {
        private readonly Speakers targetSpeaker;
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
        private readonly Dictionary<Speakers, Func<SampleChannelMapping, Func<Sample[], Sample[]>>> converterResolver = new Dictionary<Speakers, Func<SampleChannelMapping, Func<Sample[], Sample[]>>>()
        {
            { Speakers.Mono, x => x.ToMono },
            { Speakers.OnePointOne, x => x.ToOnePointOne },
            { Speakers.Stereo, x => x.ToStereo },
            { Speakers.TwoPointOne, x => x.ToTwoPointOne },
            { Speakers.ThreePointZero, x => x.ToThreePointZero },
            { Speakers.ThreePointOne, x => x.ToThreePointOne },
            { Speakers.Quad, x => x.ToQuad },
            { Speakers.Surround, x => x.ToSurround },
            { Speakers.FivePointZero, x => x.ToFivePointZero },
            { Speakers.FivePointOne, x => x.ToFivePointOne },
            { Speakers.SevenPointZero, x => x.ToSevenPointZero },
            { Speakers.SevenPointOne, x => x.ToSevenPointOne },
            { Speakers.FivePointOneSurround, x => x.ToFivePointOneSurround },
            { Speakers.SevenPointOneSurround, x => x.ToSevenPointOneSurround }
        };

        public ChannelAudioSampler(Speakers targetSpeaker)
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
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            // TODO: check if channel config is allready matching
            var targetChannels = ChannelFactory.GetChannels(targetSpeaker);
            var factor = targetChannels / (double) audio.FmtSubChunk.Channels;
            var newSize = (uint) (factor * audio.DataSubChunk.ChunkSize);

            return Task.FromResult(audio
                    .WithFmtSubChunk(x => x
                        .WithChannels(targetChannels))
                    .WithDataSubChunk(x => x
                        .WithChunkSize(newSize)
                        .WithData(ManipulateAudioData(audio, cancellationToken))));
        }

        [return: NotNull]
        private async IAsyncEnumerable<Sample> ManipulateAudioData([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull] [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            //TODO: better way to get source channels
            Speakers sourceSpeaker = ChannelFactory.GetDefaultMapping(audio.FmtSubChunk.Channels);
            var channelMapping = channelMappings.First(x => x.Speaker == sourceSpeaker);
            var samples = audio.GetAudioSamplesAsync(cancellationToken);
            var converter = converterResolver[targetSpeaker].Invoke(channelMapping);
            var temp = new Sample[audio.FmtSubChunk.Channels];
            var targetChannels = ChannelFactory.GetChannels(targetSpeaker);

            var counter = 0;
            await foreach (var value in samples.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                temp[counter] = value;
                if (++counter == audio.FmtSubChunk.Channels)
                {
                    var convertedSample = converter.Invoke(temp);
                    foreach(var sample in convertedSample)
                    {
                        yield return sample;
                    }
                    counter = 0;
                }
            }
        }
    }
}
