namespace NtFreX.Audio.PulseAudio
{
    //public class PulseAudioDevice : IAudioDevice
    //{
    //    private readonly List<PulseAudioPlaybackContext> playbackContexts = new List<PulseAudioPlaybackContext>();

    //    public async Task<IPlaybackContext> PlayAsync(IWaveAudioContainer streamAudioContainer, CancellationToken cancellationToken = default)
    //    {
    //        var spec = new pa_sample_spec
    //        {
    //            channels = (byte) streamAudioContainer.FmtSubChunk.NumChannels,
    //            format = pa_sample_format.PA_SAMPLE_U8, //TODO: set from container
    //            rate = streamAudioContainer.FmtSubChunk.SampleRate
    //        };

    //        var error = 0;
    //        var client = Simple.pa_simple_new(null, "NtFreX.Audio", pa_stream_direction.PA_STREAM_PLAYBACK, null, "Unknown", spec, null, null, ref error);
    //        if (error != 0) throw new Exception(error.ToString());

    //        var context = new PulseAudioPlaybackContext(client, streamAudioContainer.GetAudioSamplesAsync(cancellationToken), streamAudioContainer.FmtSubChunk.ByteRate, cancellationToken);
    //        playbackContexts.Add(context);
    //        return await Task.FromResult(context).ConfigureAwait(false);
    //    }

    //    public void Dispose()
    //    {
    //        foreach(var context in playbackContexts)
    //        {
    //            context.Dispose();
    //        }
    //    }

    //    public bool IsInitialized()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool TryInitialize(IWaveAudioContainer audio, out Format supportedFormat)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    [return: NotNull]
    //    public Task<IPlaybackContext> PlayAsync(CancellationToken cancellationToken = default)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class PulseAudioPlaybackContext: IPlaybackContext
    //{
    //    private readonly pa_simple client;
    //    private readonly IAsyncEnumerable<byte[]> data;
    //    private readonly uint bufferSize;
    //    private readonly CancellationToken cancellationToken;
    //    private readonly Task task;

    //    public Observable<EventArgs> EndOfDataReached { get; } = new Observable<EventArgs>();

    //    internal PulseAudioPlaybackContext(pa_simple client, IAsyncEnumerable<byte[]> data, uint bufferSize, CancellationToken cancellationToken = default)
    //    {
    //        this.client = client;
    //        this.data = data;
    //        this.bufferSize = bufferSize;
    //        this.cancellationToken = cancellationToken;

    //        task = Task.Run(PumpAudioAsync, cancellationToken);
    //    }

    //    private async Task<byte[]> FillNextBufferAsync()
    //    {
    //        var buffer = new byte[bufferSize];
    //        var bufferIndex = 0;
    //        await foreach (var value in data.WithCancellation(cancellationToken).ConfigureAwait(false))
    //        {
    //            foreach (var byteValue in value)
    //            {
    //                for (; bufferIndex < buffer.Length; bufferIndex++)
    //                {
    //                    buffer[bufferIndex] = byteValue;
    //                }

    //                if (bufferIndex == buffer.Length)
    //                {
    //                    return buffer;
    //                }
    //            }
    //        }

    //        return new byte[0];
    //    }

    //    private async Task PumpAudioAsync()
    //    {
    //        //TODO: loop over data
    //        var buffer = await FillNextBufferAsync().ConfigureAwait(false);
    //        var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
    //        IntPtr dataPtr = handle.AddrOfPinnedObject();

    //        var error = 0;
    //        Simple.pa_simple_write(client, dataPtr, (uint) buffer.Length, ref error);
    //        if (error != 0) throw new Exception(error.ToString());

    //        Simple.pa_simple_drain(client, ref error);
    //        if (error != 0) throw new Exception(error.ToString());

    //        EndOfDataReached?.Invoke(this, EventArgs.Empty);
    //    }

    //    public void Dispose()
    //    {
    //        task.Dispose();
    //        Simple.pa_simple_free(client);
    //    }
    //}
}
