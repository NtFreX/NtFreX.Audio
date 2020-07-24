using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Wasapi.Interop;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Wasapi.Wrapper
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/coreaudio/capturing-a-stream
    /// </summary>
    internal class ManagedAudioCapture : IDisposable
    {
        public const int RefTimesPerSec = 10000000;
        public const int RefTimesPerMilisec = 10000;

        private readonly ManagedWaveFormat managedWaveFormat;
        private readonly ManagedAudioClient managedAudioClient;
        private readonly IAudioCaptureClient audioCaptureClient;
        private readonly IAudioSink sink;
        private readonly CancellationToken cancellationToken;
        private readonly Task audioPump;
        private readonly uint bufferFrameCount;

        private bool isDisposed = false;

        internal ManagedAudioCapture(ManagedAudioClient managedAudioClient, IAudioCaptureClient audioCaptureClient, IAudioSink sink, CancellationToken cancellationToken)
        {
            this.bufferFrameCount = managedAudioClient.GetBufferSize();
            this.managedWaveFormat = managedAudioClient.GetMixFormat();
            this.managedAudioClient = managedAudioClient;
            this.audioCaptureClient = audioCaptureClient;
            this.sink = sink;
            this.cancellationToken = cancellationToken;

            audioPump = Task.Run(PumpAudioAsync, cancellationToken);
        }

        public AudioFormat GetFormat() => managedWaveFormat.ToAudioFormat();

        public void Dispose()
        {
            managedWaveFormat.Dispose();
            managedAudioClient.Stop();
            Marshal.ReleaseComObject(audioCaptureClient);
            _ = audioPump.ContinueWith(x => x.Dispose(), TaskScheduler.Default);
            isDisposed = true;
        }

        private async Task PumpAudioAsync()
        {
            var hnsActualDuration = (double)RefTimesPerSec * bufferFrameCount / managedWaveFormat.Unmanaged.Format.SamplesPerSec;

            managedAudioClient.Start();

            while(!isDisposed)
            {
                // Sleep for half the buffer duration.
                await Task.Delay((int)(hnsActualDuration / RefTimesPerMilisec / 2), cancellationToken).ConfigureAwait(false);

                audioCaptureClient.GetNextPacketSize(out var numFramesInNextPackage).ThrowIfNotSucceded();

                while (numFramesInNextPackage != 0)
                {
                    // Get the available data in the shared buffer.
                    audioCaptureClient.GetBuffer(
                        out var dataPtr,
                        out var numFramesToRead,
                        out var flags,
                        out _,
                        out _).ThrowIfNotSucceded();

                    if (flags.HasFlag(AudioClientBufferFlags.Silent))
                    {
                        // Tell CopyData to write silence.
                        dataPtr = IntPtr.Zero;  
                    }

                    // Copy the available capture data to the audio sink.
                    var dataLength = numFramesToRead * managedWaveFormat.Unmanaged.Format.BlockAlign;
                    var buffer = new byte[dataLength];
                    Marshal.Copy(dataPtr, buffer, 0, (int) dataLength);

                    sink.DataReceived(buffer);

                    audioCaptureClient.ReleaseBuffer(numFramesToRead).ThrowIfNotSucceded();

                    audioCaptureClient.GetNextPacketSize(out numFramesInNextPackage).ThrowIfNotSucceded();
                }
            }
        }
    }
}
