using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Container;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Extensions
{
    public static class AudioDeviceExtensions
    {
        public static async Task<IRenderContext> RenderAsync(this IAudioDevice device, IAudioContainer audio, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));
            _ = device ?? throw new ArgumentNullException(nameof(device));

#pragma warning disable CA2000 // Dispose objects before losing scope => IRenderContext wraps the client and disposes it
            if (!device.TryInitialize(audio.GetFormat(), out IAudioClient? audioClient, out var supportedFormat) || audioClient == null)
            {
                audio = await audio.ToFormatAsync(supportedFormat, cancellationToken).ConfigureAwait(false);

                if (!device.TryInitialize(audio.GetFormat(), out audioClient, out _) || audioClient == null)
                {
                    throw new Exception("The given audio is not supported");
                }
            }
#pragma warning restore CA2000 // Dispose objects before losing scope

            return await audioClient.RenderAsync(audio, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<ICaptureContext> CaptureAsync(this IAudioDevice device, AudioFormat format, IAudioSink sink, CancellationToken cancellationToken = default)
        {
            _ = device ?? throw new ArgumentNullException(nameof(device));

#pragma warning disable CA2000 // Dispose objects before losing scope => ICaptureContext wraps the client and disposes it
            if (!device.TryInitialize(format, out var audioClient, out _) || audioClient == null)
            {
                throw new Exception("The given format is not supported");
            }
#pragma warning restore CA2000 // Dispose objects before losing scope

            return await audioClient.CaptureAsync(sink, cancellationToken).ConfigureAwait(false);
        }
    }
}
